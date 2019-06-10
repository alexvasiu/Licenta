export interface Song {
    id: number;
    name: string;
    artist: string;
    genre: string;
    apparitionDate: Date;
    identificationCounter : number;
    youtubeLink: string;
    spotifyLink: string;
    beatPortLink: string;
    duration : number;
    picture: any[];
}