import React, { Component } from 'react';
import { MusicStoreContext } from '../Context/Context';
import { View, StyleSheet, Text, Image, Modal, ActivityIndicator, Dimensions } from 'react-native';
import AudioRecord from 'react-native-audio-record';
import Permissions from 'react-native-permissions';
import { SongService } from './SongService';
import { Song } from './Song';
import {Buffer} from 'buffer';
import { Card, Button, Icon } from 'react-native-elements';
import { SongInfo } from './SongInfo';
import { AddToPlaylist } from './AddToPlaylist';

import {
    PieChart
  } from 'react-native-chart-kit'

interface Props {
    navigation: any;
}

interface States {
    currState: number;
    audioTempFile: string;
    gifState: number;
    foundedSong?: Song;
    buttonEnabled: boolean;
    displaySong: boolean;
    modalVisible: boolean;
    loadedClassify: boolean;
    classifyData: any[];
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
            displaySong: false,
            modalVisible: false,
            loadedClassify: true,
            classifyData: []
        }
    }

    async componentDidMount()
    {
        await this.checkPermission();

        const options = {
            sampleRate: 11025,  // default 44100
            channels: 1,        // 1 or 2, default 1
            bitsPerSample: 16,  // 8 or 16, default 16
            audioSource: 6,     // android only (see below)
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

    getRandomColor() {
        var letters = '0123456789ABCDEF';
        var color = '#';
        for (var i = 0; i < 6; i++) {
          color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
      }

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
                                     ?
                                     <React.Fragment>
                                        <Text style={{fontSize: 30, color: 'red'}}>No results found</Text>
                                        {
                                            this.state.classifyData.length == 0 && this.state.loadedClassify ? 
                                            <Button title="Classify Song" onPress={() => {
                                                this.setState({loadedClassify: false, classifyData: []}, () => {
                                                    SongService.clasifySongData(this.state.audioTempFile, data.user.token)
                                                    .then((data: any[]) => {
                                                        console.warn(data)
                                                        this.setState({loadedClassify: true, classifyData: data.map(x => {
                                                            return {
                                                                name: x.item1,
                                                                test: x.item2,
                                                                legendFontSize: 15,
                                                                color: this.getRandomColor()
                                                            }
                                                        })})
                                                    })
                                                })
                                            }}/>
                                            :
                                            ! this.state.loadedClassify ? 
                                                <ActivityIndicator size='large' />
                                            : this.state.classifyData.length == 10 ?
                                            <React.Fragment>
                                                <PieChart
                                                    data={this.state.classifyData}
                                                    width={Dimensions.get('window').width}
                                                    height={220}
                                                    backgroundColor="transparent"
                                                    paddingLeft="15"
                                                    accessor="test"
                                                    chartConfig={{
                                                        backgroundGradientFrom: '#1E2923',
                                                        backgroundGradientTo: '#08130D',
                                                        color: (opacity = 1) => `rgba(26, 255, 146, ${opacity})`,
                                                        strokeWidth: 2 // optional, default 3
                                                    }}
                                                />
                                            </React.Fragment> : null
                                        }
                                       
                                     </React.Fragment> :
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
                                        title='See more info'
                                        onPress={() => {
                                            this.setState({modalVisible: true})
                                        }} />
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
                                            this.setState({loadedClassify: true, classifyData: [], audioTempFile: '', displaySong: false}, AudioRecord.start());
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
                        <Modal
                            animationType="slide"
                            transparent={false}
                            visible={this.state.modalVisible}
                            onRequestClose={() => {
                                this.setState({modalVisible: false})
                            }}>
                            <View style={{marginTop: 22}}>
                                <View>
                                    {this.state.foundedSong != undefined ?
                                       <React.Fragment>
                                            <SongInfo song={this.state.foundedSong} /> 
                                            <AddToPlaylist songId={this.state.foundedSong.id} />
                                       </React.Fragment>
                                    : null}
                                </View>
                            </View>
                        </Modal>
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