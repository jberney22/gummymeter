﻿namespace GummyMeter.Models
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterPath { get; set; }
        public double VoteAverage { get; set; }
        public string ReleaseDate { get;  set; } = string.Empty;
        public string Overview { get;  set; } = string.Empty;

        public List<string> Genres { get; set; } = new();
        public string GenresFormatted { get; set; } = string.Empty;
        public string MPARating { get;  set; }
        public string Runtime { get;  set; }
        public int? ReviewCount { get;  set; }

        public int FakeTomatometer { get; set; }
        public int FakePopcornmeter { get; set; }
    }

    public class MovieViewModel2
    {
        public List<MovieViewModel>? TrendingMovies { get; set; } = new List<MovieViewModel>();
        public List<MovieViewModel>? PopularMovies { get; set; } = new List<MovieViewModel>();
        public List<MovieViewModel>? TopRatedMovies { get; set; } = new List<MovieViewModel>();
        public List<MovieViewModel>? UpcomingMovies { get;  set; } = new List<MovieViewModel>();
        public List<MovieViewModel>? NowPlayingMovies { get;  set; } = new List<MovieViewModel>();
    }

    public class MovieDetailViewModel
    {
        public int MovieId { get; set; }
        public MovieViewModel Movie { get; set; }
        public List<Review> Reviews { get; set; }
        public List<CastMember> Cast { get; set; } = new();
        public string Director { get; set; } = string.Empty;

        public string? TrailerYoutubeKey { get; set; } // only for YouTube trailers

        public List<string> Writers { get; set; } = new();
        public List<Rating> Ratings { get; internal set; }
        public double Avg { get; internal set; }
        public int? UserRating { get;  set; }
        public List<CastMember> Producers { get; internal set; }
        public double Tomatometer { get;  set; }
        public double Popcornmeter { get;  set; }
    }

    public class CastMember
    {
        public string Name { get; set; } = string.Empty;
        public string Character { get; set; } = string.Empty;
        public string? ProfilePath { get; set; }
    }
}
