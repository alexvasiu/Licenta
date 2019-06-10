import React, { Component } from 'react';
import { MusicStoreContext } from '../Context/Context';
import { View, StyleSheet, Text, Image } from 'react-native';
import AudioRecord from 'react-native-audio-record';
import Permissions from 'react-native-permissions';
import { SongService } from './SongService';
import { Song } from './Song';
import {Buffer} from 'buffer';
import { Card, Button, Icon } from 'react-native-elements';

var RNFS = require('react-native-fs');

interface Props {
}

interface States {
    currState: number;
    audioTempFile: string;
    gifState: number;
    foundedSong?: Song;
    buttonEnabled: boolean;
    displaySong: boolean;
}

export class MainView extends Component<Props, States> {
    constructor(props: Props) {
        super(props);
        this.state = {
            currState: 0,
            audioTempFile: '',
            gifState: 0,
            foundedSong: undefined,
            buttonEnabled: true,
            displaySong: false
        }
    }

    async componentDidMount()
    {
        await this.checkPermission();

        const options = {
            sampleRate: 44100,  // default 44100
            channels: 2,        // 1 or 2, default 1
            bitsPerSample: 16,  // 8 or 16, default 16
            audioSource: 1,     // android only (see below)
            wavFile: 'test.wav' // default 'audio.wav'
          };

          AudioRecord.init(options);
    }

    checkPermission = async () => {
        let permissions = ['microphone'];
        const p = await Permissions.checkMultiple(permissions);
        let toRequest: string[] = [];
        permissions.forEach(x => {
            if (p[x] !== 'authorized')
                toRequest.push(x);
        })
        if (toRequest.length == 0) return;
        return this.requestPermission(toRequest);
    };

    requestPermission = async (what: string[]) => {
        let p;
        for (let x of what)
            p = await Permissions.request(x);
    };

    render() {
        return (
            <MusicStoreContext.Consumer>
                {(data: any) => (
                    <View style={styles.container}>
                        <View style={styles.container2}>
                            {
                                this.state.gifState == 0 ? this.state.displaySong ?
                                <React.Fragment>
                                    {this.state.foundedSong == undefined 
                                    || this.state.foundedSong == null
                                     ? <Text style={{fontSize: 30, color: 'red'}}>No results found</Text> :
                                <Card
                                    containerStyle = {{width: '95%'}}
                                    title={this.state.foundedSong.name}
                                    image={this.state.foundedSong.picture == null ? require('../../images/unknown.png') 
                                    : {uri: 'data:image/png;base64,' + Buffer.from(this.state.foundedSong.picture).toString('base64')} }>
                                    <View style={{marginBottom: 10}}>
                                       <Text>Artist: {this.state.foundedSong.artist}</Text>
                                       <Text>Genre: {this.state.foundedSong.genre}</Text>
                                    </View>
                                    <Button
                                        icon={<Icon name='info' color='#ffffff' />}
                                        buttonStyle={{backgroundColor: '#03A9F4', borderRadius: 0, marginLeft: 0, marginRight: 0, marginBottom: 0}}
                                        title='See more info' />
                                </Card>}
                                </React.Fragment>
                                : null
                                : this.state.gifState == 1 ?
                                <Image
                                style={{width: 300, height: 300}}
                                source={require('../../images/_2.gif')} /> :
                                <Image 
                                style={{width: 300, height: 300}}
                                source={require('../../images/_1.gif')} />
                            }
                        </View>
                        <View style={ styles.bottomView} >
                            <Button style={styles.button} disabled={!this.state.buttonEnabled}
                                onPress={ async () => {
                                        let curr = this.state.currState;
                                        this.setState({currState: (curr + 1) % 3, gifState: curr == 0 ? 1 : curr == 1 ? 0 : 2, buttonEnabled: curr != 2}, async () => {
                                            if (curr == 0)
                                            this.setState({audioTempFile: '', displaySong: false}, AudioRecord.start());
                                            else if (curr == 1)
                                                this.setState({audioTempFile: await AudioRecord.stop()});
                                            else {
                                                SongService.analyze(this.state.audioTempFile, data.user.token).then((song: Song) => {
                                                    this.setState({gifState: 0, displaySong: true, foundedSong: song.name == undefined ? undefined: song, buttonEnabled: true})
                                                }, (err: any) => {
                                                    this.setState({gifState: 0, displaySong: true, foundedSong: undefined, buttonEnabled: true})
                                                })
                                                .catch(() => {
                                                    this.setState({gifState: 0, displaySong: true, foundedSong: undefined, buttonEnabled: true})
                                                })
                                            }
                                        })
                                    }} title={this.state.currState == 0 ? 
                                        "Start Recording" : this.state.currState == 1 ? "Stop Recording" :
                                            "Search Song"} />
                        </View>
                    </View>
                )}
            </MusicStoreContext.Consumer>
        )
    }
}

const styles = StyleSheet.create({
    container: {
        justifyContent: 'center',
        flexDirection: 'column',
        flex: 1,
        padding: 20,
        alignItems: 'center'
    },
    container2: {
        width: '100%',
        flex: 1,
        alignItems: 'center'
    },
    button: {
        alignSelf: 'flex-end',
        position: 'absolute',
        bottom: 35
    },
    bottomView:{
        width: '100%', 
        height: 75, 
        justifyContent: 'center', 
        alignItems: 'center',
        position: 'absolute',
        bottom: 0
    }
});