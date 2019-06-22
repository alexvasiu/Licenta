import React, { Component } from 'react';
import { Song } from './Song';
import { View, StyleSheet, Image, TouchableOpacity } from 'react-native';
import { Text } from 'react-native-elements';
import MaterialIcons from 'react-native-vector-icons/MaterialIcons';
import FontAwesome from 'react-native-vector-icons/FontAwesome';
import {Buffer} from 'buffer';

interface States {

}

interface Props {
    song: Song
}

export class SongInfo extends React.Component<Props, States> {
    constructor(props: Props) 
    {
        super(props);
        this.state = {

        }
    }

    render = () => {
        let imgSource = {};
        if (this.props.song.picture == null || 
            this.props.song.picture == undefined ||
            this.props.song.picture.length == 0)
            imgSource = require('../../images/unknown.png');
        else {
            imgSource = {uri: 'data:image/png;base64,' + 
            Buffer.from(this.props.song.picture).toString('base64')};
        }
        return (
            <View style={{
                display: "flex",
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center'
            }}>
                <Image 
                source={imgSource}
                 style={{width: 200, height: 200, borderRadius: 20}} 
                />
                <Text style={styles.title}>Name: {this.props.song.name}</Text>
                <Text style={styles.artist}>Artist: {this.props.song.artist}</Text>
                <Text style={styles.artist}>Genre: {this.props.song.genre}</Text>
                <Text style={styles.artist}>Apparition Date: {new Date(this.props.song.apparitionDate).toDateString()}</Text>
                <Text style={styles.artist}>Duration: {this.props.song.duration}</Text>
                <Text style={styles.artist}>Identified {this.props.song.identificationCounter} times</Text>
                <View style={{
                     display: "flex",
                     flexDirection: 'row',
                     justifyContent: 'center',
                     alignItems: 'center',
                     marginTop: 20
                }}>
                     <FontAwesome name='youtube' size={25} style={{
                        color: 'tomato',
                        justifyContent: 'center',
                        alignItems: 'center',
                        flex: 0.2
                    }} onPress={() => {
                        
                    }}/>

                    <FontAwesome name='spotify' size={25} style={{
                        color: 'tomato',
                        justifyContent: 'center',
                        alignItems: 'center',
                        flex: 0.2
                    }} onPress={() => {
                        
                    }}/>

                    <TouchableOpacity onPress={() => {}} style={{
                        flex: 0.2
                    }}>
                        <Text>Buy Song</Text>
                    </TouchableOpacity>
                </View>
            </View>
        )
    }
}

const styles = StyleSheet.create({
    allCards: {
        display: 'flex',
        flexDirection: 'column',

    },
    card: {
        justifyContent: 'center',
        alignItems: 'center',
        display: 'flex',
        flexDirection: 'row',
        marginBottom: 20,
        marginTop: 10,
        borderStyle: 'solid',
        borderColor: 'black',
        borderWidth: 1,
        borderRadius: 25,
        height: 120
    },
    title: {
        fontSize: 22,
        backgroundColor: 'transparent',
        justifyContent: 'center',
        alignItems: 'center'
    },
    artist: {
        fontSize: 19,
        backgroundColor: 'transparent',
        justifyContent: 'center',
        alignItems: 'center'
    },
    button: {
        marginRight: 10
    }
});