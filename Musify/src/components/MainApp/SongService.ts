import {BASE_URL} from './../AppResource';
import {Song} from './Song';
import { Playlist } from './Playlist';

var RNFS = require('react-native-fs');

export class SongService {
    private static URL: string = BASE_URL + "song/";

    public static analyze(file: string, token: string) : Promise<Song> {
        return new Promise(async (resolve, reject) => {
        fetch(this.URL + "analyzeData", {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: 'Bearer ' + token
                },
                body: JSON.stringify({
                    content: await RNFS.readFile(file, 'base64'),
                    contentType: 'audio/wav'
                })
            }).then((data: any) => {
                console.warn(data)
                console.warn(data.error)
                resolve(data.json())
            }, (error) => {
                console.warn(error)
                reject(error)
            })
            .catch((error: any) => {
                console.warn(error)
                reject(error)}
            )
        })
    }

    public static addPlaylist(newPlaylist: Playlist, token: string) : Promise<Playlist> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + 'add/playlist', {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: 'Bearer ' + token
                },
                body: JSON.stringify(newPlaylist)
            })
            .then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                reject()
            }, (err) => reject(err))
            .catch(err => reject(err))
        })
    }

    public static addSongToPlaylist(playlistId: number, songId: number, token: string) : Promise<boolean> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + 'add/playlist/song', {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: 'Bearer ' + token
                },
                body: JSON.stringify({
                    playlistId,
                    songId
                })
            })
            .then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                reject()
            }, (err) => reject(err))
            .catch(err => reject(err))
        })
    }

    public static getPlaylists(songId: number, userId: number, token: string) : Promise<Playlist[]> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + 'getPlaylists', {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: 'Bearer ' + token
                },
                body: JSON.stringify({
                    songId: songId,
                    userId: userId
                })
            })
            .then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                reject()
            }, (err) => reject(err))
            .catch(err => reject(err))
        })
    }

    public static getSongsFromPlaylist(playlistId: number, token: string) : Promise<Song[]> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + 'getSongsFromPlaylist', {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: 'Bearer ' + token
                },
                body: JSON.stringify({playlistId})
            })
            .then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                reject()
            }, (err) => reject(err))
            .catch(err => reject(err))
        })
    }

    public static clasifySongData(file: string, token: string) : Promise<any[]> {
        return new Promise(async (resolve, reject) => {
        fetch(this.URL + "clasifySongData", {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: 'Bearer ' + token
                },
                body: JSON.stringify({
                    content: await RNFS.readFile(file, 'base64'),
                    contentType: 'audio/wav'
                })
            }).then((data: any) => {
                console.warn(data)
                console.warn(data.error)
                resolve(data.json())
            }, (error) => {
                console.warn(error)
                reject(error)
            })
            .catch((error: any) => {
                console.warn(error)
                reject(error)}
            )
        })
    }
}