﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#region Additional Namespaces

using ChinookSystem.DAL;
using ChinookSystem.Entities;
using ChinookSystem.ViewModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;

#endregion

namespace ChinookSystem.BLL
{
    public class PlaylistTrackServices
    {
        #region Constructor and Context Dependency

        private readonly ChinookContext _context;

        //obtain the context link from IServiceCollection when this 
        //      set of services is injected into the  "outside user"

        internal PlaylistTrackServices(ChinookContext context)
        {
            _context = context;
        }

        #endregion

        #region Queries

        public List<PlaylistTrackInfo> PlaylistTrack_Fetch_Tracks(string playlistname, string username)
        {

            IEnumerable<PlaylistTrackInfo> info = _context.PlaylistTracks
                    .Where(x => x.Playlist.Name.Equals(playlistname) && x.Playlist.UserName.Equals(username))
                    .Select(x => new PlaylistTrackInfo
                    {
                        TrackId = x.TrackId,
                        TrackNumber = x.TrackNumber,
                        SongName = x.Track.Name,
                        Millseconds = x.Track.Milliseconds

                    })
                    .OrderBy(x => x.TrackNumber);

            return info.ToList();

        }

        #endregion

        #region Commands

        public void PlaylistTrack_AddTrack(string playlistname, string username, int trackid)
        {
            //create local variables

            Track trackExist = null;
            Playlist playlistExists = null;
            PlaylistTrack playlistTrackExists = null;
            int tracknumber = 0;

            //create a List<Exception> to contain all discovered errors

            List<Exception> errorlist = new List<Exception>();

            //Business logic
            //these are processing rules that need to be satisfied for valid data
            // rule: a track can only exist once on a playlist
            // rule: each track on a playlist is assigned a continuous track number

            // if the business rules are passed, consider the data valid, then
            // a) stage your transaction work (Adds, Updates, Deletes)
            // b) execute a SINGLE .SaveChanges() - commits to database

            //parameter validation

            if (string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("Playlist name is missing");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("User name is missing");
            }

            trackExist = _context.Tracks
                .Where(x => x.TrackId == trackid)
                .FirstOrDefault();

            if (trackExist == null)
            {
                errorlist.Add(new Exception("Selected track is no longer on file. Refresh track table."));
            }


            //business process

            playlistExists = _context.Playlists
                .Where(x => x.Name.Equals(playlistname) && x.UserName.Equals(username))
                .FirstOrDefault();

            if (playlistExists == null)
            {
                //new playlist

                playlistExists = new Playlist()
                {
                    Name = playlistname,
                    UserName = username
                };

                //stage (only in memory)

                _context.Playlists.Add(playlistExists);
                tracknumber = 1;

            }
            else
            {
                //playlist already exists
                // rule: unique tracks on playlist

                playlistTrackExists = _context.PlaylistTracks
               .Where(x => x.Track.Name.Equals(playlistname) && x.Playlist.UserName.Equals(username) && x.TrackId == trackid)
               .FirstOrDefault();

                if (playlistTrackExists != null)
                {
                    var songname = _context.Tracks
                        .Where(x => x.TrackId == trackid)
                        .Select(x => x.Name)
                        .SingleOrDefault();

                    //rule violation
                    errorlist.Add(new Exception($"Selected track ({songname}) is already on the playlist."));
                }
                else
                {
                    tracknumber = _context.PlaylistTracks
                        .Where(x => x.Playlist.Name.Equals(playlistname) && x.Playlist.UserName.Equals(username))
                        .Count();
                    tracknumber++;
                }

            }

            //Add the track to the playlist
            //create an instance for the playlist track

            playlistTrackExists = new PlaylistTrack();

            //load values

            playlistTrackExists.TrackId = trackid;
            playlistTrackExists.TrackNumber = tracknumber;

            //?? what about the second part of the primary key: PlayListId?
            //if the playlist exists then we know the id: playlistExists.PlaylistId
            //BUT if the playlist is NEW, we DO NOT know the id

            // in the situation of the NEW playlist, even though we have
            // created the playlist instance (see above) it is ONLY staged!!!!

            //this means that the actual sql record has NOT yet been created
            //this means that the IDENTITY value for the new playlist DOES NOT yet Exist.
            //The value on the playlist instance (playlistExists) is zero (0). 
            //therefore we have a serious problem

            //Solution
            //It is built into EnityFramework software and is based on using the
            // navigational property in  PlayList pointing to it's "child"

            //stagin a typlical Add in the past was t reference the entity
            // and use the entity.Add(xxx)
            //  _context.PlaylistTrack.Add(playlistTrackExists)
            // If you use this statement the playlistid would be zero(0)
            // causing the transaction to ABORT
            //why? pkeys cannot be zero (0) (FKey to Pkey problem)

            //INSTEAD, do the staging using the "parent.navchildproperty.Add(xxxx)

            playlistExists.PlaylistTracks.Add(playlistTrackExists);

            //Staging is complete
            //Commit the work (transaction)
            //commiting the work need a .SaveChanges()
            //a transaction has ONLY ONE .SaveChanges()
            //BUT what if you have discovered errors during the business process???
            // if so, then throw all error and DO NOT COMMIT!!!!

            if (errorlist.Count > 0)
            {
                //throw the list of business processing error(s)

                throw new AggregateException("Unable to add new track. Check concerns", errorlist);

            }
            else
            {
                //consider data valid
                //has passed business processing rules

                _context.SaveChanges();

            }

        }

        public void PlayListTrack_RemoveTracks(string playlistname, string username,
             List<PlaylistTrackMove> trackstoremove)
        {
            Track trackExists = null;
            Playlist playlistExists = null;
            PlaylistTrack playlisttrackExists = null;
            int tracknumber = 0;
            List<Exception> errorlist = new List<Exception>();

            if (string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("Playlist name is missing");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("User name is missing");
            }
            if (trackstoremove.Count == 0)
            {
                throw new ArgumentNullException("No track list has been supplied");
            }
            playlistExists = _context.Playlists
                           .Where(x => x.Name.Equals(playlistname)
                                   && x.UserName.Equals(username))
                           .FirstOrDefault();

            if (playlistExists == null)
            {
                errorlist.Add(new Exception("Play list does not exist."));
            }
            else
            {
                IEnumerable<PlaylistTrackMove> removelist = trackstoremove
                                                            .Where(x => x.SelectedTrack);
                IEnumerable<PlaylistTrackMove> keeplist = trackstoremove
                                                           .Where(x => !x.SelectedTrack)
                                                           .OrderBy(x => x.TrackNumber);
                foreach (PlaylistTrackMove track in removelist)
                {
                    playlisttrackExists = _context.PlaylistTracks
                                .Where(x => x.Playlist.Name.Equals(playlistname)
                                        && x.Playlist.UserName.Equals(username)
                                        && x.TrackId == track.TrackId)
                                .FirstOrDefault();
                    if (playlisttrackExists != null)
                    {
                        _context.PlaylistTracks.Remove(playlisttrackExists);
                    }
                    //if the track does not exist, then there is actually no problem
                    //      because we were going to delete the track anyways.
                }
                tracknumber = 1;
                foreach (PlaylistTrackMove track in keeplist)
                {
                    playlisttrackExists = _context.PlaylistTracks
                               .Where(x => x.Playlist.Name.Equals(playlistname)
                                       && x.Playlist.UserName.Equals(username)
                                       && x.TrackId == track.TrackId)
                               .FirstOrDefault();
                    if (playlisttrackExists != null)
                    {
                        playlisttrackExists.TrackNumber = tracknumber;
                        EntityEntry<PlaylistTrack> updating = _context.Entry(playlisttrackExists);
                        updating.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        tracknumber++;
                    }
                    else
                    {
                        var songname = _context.Tracks
                                   .Where(x => x.TrackId == track.TrackId)
                                   .Select(x => x.Name)
                                   .SingleOrDefault();
                        errorlist.Add(new Exception($"Track {songname} is no longer on playlist. Refresh search and repeat remove"));
                    }
                }
            }
            if (errorlist.Count > 0)
            {
                //throw the list of business processing error(s)
                throw new AggregateException("Unable to remove tracks. Check concerns", errorlist);
            }
            else
            {
                //consider data valid
                //has passed business processing rules
                _context.SaveChanges();
            }
        }

        public void PlayListTrack_MoveTracks(string playlistname, string username,
                    List<PlaylistTrackMove> trackstomove)
        {
            Track trackExists = null;
            Playlist playlistExists = null;
            PlaylistTrack playlisttrackExists = null;
            int tracknumber = 0;
            List<Exception> errorlist = new List<Exception>();

            if (string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("Playlist name is missing");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("User name is missing");
            }
            if (trackstomove.Count == 0)
            {
                throw new ArgumentNullException("No track list has been supplied");
            }
            playlistExists = _context.Playlists
                           .Where(x => x.Name.Equals(playlistname)
                                   && x.UserName.Equals(username))
                           .FirstOrDefault();

            if (playlistExists == null)
            {
                errorlist.Add(new Exception("Play list does not exist."));
            }
            else
            {
                //code to re-sequence a playlist
                //sort the command model data list on the re-org value
                trackstomove.Sort((x, y) => x.TrackInput.CompareTo(y.TrackInput));
                //validation 
                //  a) numeric and positive non-zero

                int tempNum = 0;
                foreach (var track in trackstomove)
                {
                    var songname = _context.Tracks
                                   .Where(x => x.TrackId == track.TrackId)
                                   .Select(x => x.Name)
                                   .SingleOrDefault();
                    if (int.TryParse(track.TrackInput, out tempNum))
                    {
                        if (tempNum < 1)
                        {

                            errorlist.Add(new Exception($"{songname} re-sequence value needs to be greater than 0. Example 3"));
                        }
                    }
                    else
                    {
                        errorlist.Add(new Exception($"{songname} re-sequence value needs to be a number. Example: 3"));
                    }
                }
                //  b) unique new track numbers

                for (int i = 0; i < trackstomove.Count - 1; i++)
                {
                    if (!string.IsNullOrWhiteSpace(trackstomove[i].TrackInput) &&
                        !string.IsNullOrWhiteSpace(trackstomove[i - 1].TrackInput) )
                    {

                    }

                    var songname1 = _context.Tracks
                                   .Where(x => x.TrackId == trackstomove[i].TrackId)
                                   .Select(x => x.Name)
                                   .SingleOrDefault();
                    var songname2 = _context.Tracks
                                   .Where(x => x.TrackId == trackstomove[i + 1].TrackId)
                                   .Select(x => x.Name)
                                   .SingleOrDefault();
                    if (trackstomove[i] == trackstomove[i + 1])
                    {
                        errorlist.Add(new Exception($"{songname1} and {songname2} have the same re-sequence number. Re-sequence number must be unique."));

                    }
                }
                //Re-sequence track numbers
                tracknumber = 1;
                foreach (PlaylistTrackMove track in trackstomove)
                {
                    playlisttrackExists = _context.PlaylistTracks
                               .Where(x => x.Playlist.Name.Equals(playlistname)
                                       && x.Playlist.UserName.Equals(username)
                                       && x.TrackId == track.TrackId)
                               .FirstOrDefault();
                    if (playlisttrackExists != null)
                    {
                        playlisttrackExists.TrackNumber = tracknumber;
                        EntityEntry<PlaylistTrack> updating = _context.Entry(playlisttrackExists);
                        updating.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        tracknumber++;
                    }
                    else
                    {
                        var songname = _context.Tracks
                                   .Where(x => x.TrackId == track.TrackId)
                                   .Select(x => x.Name)
                                   .SingleOrDefault();
                        errorlist.Add(new Exception($"{songname} is no longer on playlist. Refresh search and repeat remove"));
                    }
                }
            }
            if (errorlist.Count > 0)
            {
                //throw the list of business processing error(s)
                throw new AggregateException("Unable to move tracks. Check concerns", errorlist);
            }
            else
            {
                //consider data valid
                //has passed business processing rules
                _context.SaveChanges();
            }
        }



        #endregion





    }

}
