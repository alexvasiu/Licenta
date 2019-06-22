import {BASE_URL} from './../AppResource';
import {Song} from './Song';

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
}