import React, { Component } from 'react';
import { View, FlatList, StyleSheet, Text, Button, TouchableOpacity, Modal, TouchableHighlight, ActivityIndicator} from 'react-native';
import { Playlist } from './Playlist';
import { MusicStoreContext } from '../Context/Context';
import MaterialIcons from 'react-native-vector-icons/MaterialIcons';
import FontAwesome from 'react-native-vector-icons/FontAwesome';
import { Song } from './Song';
import { SongInfo } from './SongInfo';
import { SongService } from './SongService';

interface Props {
    navigation: any;
}

interface States {
    playlists: Playlist[];
    songs: Song[];
    showSongs: boolean;
    modalVisible: boolean;
    index: number;
    loaded: boolean;
    playlistName: string;
}

export class Playlists extends Component<Props, States> {

    constructor(props: Props) {
        super(props);
        this.state = {
            playlists: [],
            showSongs: false,
            songs: [],
            modalVisible: false,
            index: -1,
            loaded: false,
            playlistName: ''
        }
    }

    componentDidMount()
    {
        console.warn(this.context.user)
        SongService.getPlaylists(-1, this.context.user.id, this.context.user.token)
        .then((playlists: Playlist[]) => {
            this.setState({playlists, loaded: true})
        })
    }

    handleMore() 
    {

    }

    renderSong(song: Song, index: number) {
        return (
            <View style={styles.card} key={song.id}>
            <MaterialIcons name="queue-music" style={{
                fontSize: 45,
                color: 'tomato',
                justifyContent: 'center',
                alignItems: 'center',
                flex: 0.2 
            }} />
             <View style={{
                display: "flex",
                flexDirection: "column",
                flex: 1
            }}>
                <Text style={styles.title}>{song.name}</Text>
                <Text style={styles.artist}>{song.artist}</Text>
                <View  style={{
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

                    <TouchableOpacity onPress={() => {
                        this.setState({modalVisible: true, index})
                    }} style={{
                        flex: 0.2
                    }}>
                        <Text>More details</Text>
                    </TouchableOpacity>

                    <TouchableOpacity onPress={() => {}} style={{
                        flex: 0.2
                    }}>
                        <Text>Buy Song</Text>
                    </TouchableOpacity>
                </View>
            </View>
        </View>
        )
    }

    renderItem(playlist: Playlist, index: number)
    {
        return (
            <View style={styles.card} key={playlist.id}>
                <MaterialIcons name="queue-music" style={{
                    fontSize: 45,
                    color: 'tomato',
                    justifyContent: 'center',
                    alignItems: 'center',
                    flex: 0.2 
                }} />
                 <View style={{
                    display: "flex",
                    flexDirection: "column",
                    flex: 1
                }}>
                    <Text style={styles.title}>{playlist.name}</Text>
                    <View  style={{
                        display: "flex",
                        flexDirection: 'row',
                        justifyContent: 'center',
                        alignItems: 'center',
                        marginTop: 20
                    }}>
                        <FontAwesome name={playlist.public ? 'unlock': 'lock'} size={25} style={{
                            color: 'tomato',
                            justifyContent: 'center',
                            alignItems: 'center',
                            flex: 0.2
                        }} onPress={() => {
                            
                        }}/>

                        <TouchableOpacity onPress={() => {
                            SongService.getSongsFromPlaylist(playlist.id, this.context.user.token)
                            .then((songs: Song[]) => {
                                this.setState({songs, showSongs: true, playlistName: playlist.name})
                            })
                        }} style={{
                            flex: 0.3
                        }}>
                            <Text>View Songs</Text>
                        </TouchableOpacity>

                        <FontAwesome name='share' size={25} style={{
                        color: 'tomato',
                        justifyContent: 'center',
                        alignItems: 'center',
                        flex: 0.2
                    }} onPress={() => {
                        
                    }}/>
                   
                    </View>
                </View>
            </View>)
    }

    render() 
    {
        return (
            <React.Fragment>
            {!this.state.loaded ? 
            <ActivityIndicator size='large' /> :
                <View>
                {this.state.showSongs ? 
                <React.Fragment>
                    <Modal
                        animationType="slide"
                        transparent={false}
                        visible={this.state.modalVisible}
                        onRequestClose={() => {
                            this.setState({index: -1, modalVisible: false})
                        }}>
                        <View style={{marginTop: 22}}>
                            <View>
                                <SongInfo song={this.state.songs[this.state.index]} />
                            </View>
                        </View>
                    </Modal>
                    <Text style={{...styles.title, 
                        alignContent: 'center',
                        justifyContent:'center'
                        }}>
                        {this.state.playlistName}
                    </Text>
                    <FlatList style={styles.allCards}
                    data={this.state.songs}
                    renderItem={({item, index}) => this.renderSong(item, index)}
                    keyExtractor={(_, index) => index.toString()}
                    onEndReached={this.handleMore}
                    onEndReachedThreshold={0.1} />
                <Button title="Back to playlists" onPress={() => {
                    this.setState({showSongs: false})
                }} />
                </React.Fragment>
                :
                <FlatList style={styles.allCards}
                data={this.state.playlists}
                renderItem={({item, index}) => this.renderItem(item, index)}
                keyExtractor={(_, index) => index.toString()}
                onEndReached={this.handleMore}
                onEndReachedThreshold={0.1}
                />}
            </View>
            }
        </React.Fragment>

        
        )
    }
}

Playlists.contextType = MusicStoreContext;

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