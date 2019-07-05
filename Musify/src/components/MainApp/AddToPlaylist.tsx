import React, { Component } from 'react';
import { ActivityIndicator, Button, ToastAndroid, Modal, View, TextInput } from 'react-native';
import { SongService } from './SongService';
import { MusicStoreContext } from '../Context/Context';
import { Playlist } from './Playlist';
import { Dropdown } from 'react-native-material-dropdown';
import { CheckBox } from 'react-native-elements';

interface Props {
    songId: number
}

interface States {
    loading: boolean;
    playlists: Playlist[];
    index: number;
    modalVisible: boolean;
    playlistName: string;
    checked: boolean;
}

export class AddToPlaylist extends React.Component<Props, States> {
    constructor(props: Props)
    {
        super(props)
        this.state = {
            loading: true,
            playlists: [],
            index: -1,
            modalVisible: false,
            playlistName: '',
            checked: false
        }
    }

    componentDidMount()
    {
        SongService.getPlaylists(this.props.songId, this.context.user.id, this.context.user.token)
        .then((playlists: Playlist[]) => 
        this.setState({playlists, loading: false, modalVisible: false}))
    }

    render = () => {
        let data : any[] = [];
        if (!this.state.loading)
        {
            data = this.state.playlists.map(x => {
                return {
                    value: x.name
                }
            })

            data.push({
                value: 'Add in new playlist'
            })
        }
        return (
            <React.Fragment>
                {
                    this.state.loading ? 
                        <ActivityIndicator size='large' />
                        :
                        <React.Fragment>
                            <Dropdown label='' data={data} onChangeText = {(args, index, d) => {
                                this.setState({index})
                            }} />
                            <Button title={data.length == 0 || this.state.index != data.length - 1 ? 
                            "Add to playlist" : "Create and add to playlist"} onPress={() => {
                                if (this.state.index >= 0)
                                {
                                    if (this.state.index == data.length - 1)
                                    {
                                        this.setState({modalVisible: true, playlistName: ''})
                                    }
                                    else
                                        SongService.addSongToPlaylist(this.state.playlists[this.state.index].id,
                                            this.props.songId, this.context.user.token)
                                            .then(() => {
                                                ToastAndroid.show("Song added to playlist", ToastAndroid.LONG)
                                            }, () => {
                                                ToastAndroid.show("Something went wrong", ToastAndroid.LONG)
                                            })
                                            .catch(() => {
                                                ToastAndroid.show("Something went wrong", ToastAndroid.LONG)
                                            })
                                }
                            }} />

                        <Modal
                            animationType="slide"
                            transparent={false}
                            visible={this.state.modalVisible}
                            onRequestClose={() => {
                                this.setState({modalVisible: false})
                            }}>
                            <View style={{marginTop: 22}}>

                                <TextInput value={this.state.playlistName} 
                                placeholder="New playlist name" 
                                onChangeText={(text) => {this.setState({playlistName: text})}}
                                />

                                <CheckBox
                                title='Public'
                                checked={this.state.checked}
                                onPress={() => this.setState({checked: !this.state.checked})}
                                />


                                <Button title={"Create playlist"} onPress={() => {
                                    SongService.addPlaylist({
                                        id: 0,
                                        userId: this.context.user.id,
                                        name: this.state.playlistName,
                                        public: this.state.checked,
                                        shareLink: ''
                                    }, this.context.user.token)
                                    .then((newPlaylist: Playlist) => {
                                        SongService.addSongToPlaylist(newPlaylist.id, this.props.songId, 
                                            this.context.user.token)
                                        .then(() => {
                                            ToastAndroid.show("Playlist created successfully", ToastAndroid.LONG)
                                            this.setState({modalVisible: false, playlistName: '', checked: false})
                                        })
                                    })
                                }} />
                            </View>
                        </Modal>
                        </React.Fragment>
                }
            </React.Fragment>
        )
    }
}

AddToPlaylist.contextType = MusicStoreContext;