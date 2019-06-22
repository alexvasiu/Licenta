import React from "react";
import { View, StyleSheet, Text, Modal, TextInput } from "react-native";
import {Button} from 'react-native-elements';
import { MusicStoreContext } from "../Context/Context";
import { LoginButton, AccessToken } from "react-native-fbsdk";
import { GoogleSigninButton } from "react-native-google-signin";

interface Props {
    navigation : any;
}

interface States {
    changeType: string;
    modalVisible: boolean;
    password1: string;
    password2: string;
    email: string;
}

export class UserInfo extends React.Component<Props, States> {
    constructor(props: Props) {
        super(props);
        this.state = {
            modalVisible: false,
            changeType: '',
            password1: '',
            password2: '',
            email: ''
        }
    }

    render = () => (
        <MusicStoreContext.Consumer>
            {(data: any) => (
                <View style={styles.container}>
                    {
                        data && data.user ?
                        <React.Fragment>
                            <Text>Username: {data.user.username}</Text>
                            <Text>Email: {data.user.email}</Text>
                            {data.user.facebookId == null || data.user.facebookId == ""? 
                             <React.Fragment>
                             <Text style={{marginTop: 20}}>Link your facebook account:</Text>
                                 <LoginButton
                                 
                                 onLoginFinished={
                                     (error, result) => {
                                     if (error) {
                                         console.warn("login has error: " + JSON.stringify(result));
                                         console.warn("login has error: " + error);
                                     } else if (result.isCancelled) {
                                         console.warn("login is cancelled.");
                                     } else {
                                         AccessToken.getCurrentAccessToken().then(
                                             (data1: any) => {
                                                 console.warn(data1)
                                                 console.warn(data1.accessToken.toString())
                                                 // LoginManager.logOut();
                                             }
                                         )
                                     }
                                     }
                                 }
                                 onLogoutFinished={() => console.warn("logout.")}/>
                                 </React.Fragment>
                                : <Text>Facebook Account Linked</Text>}

                            {data.user.googleId == null || data.user.googleId == "" ? <React.Fragment>
                                <Text style={{marginTop: 20}}>Link your google account:</Text>
                                    <GoogleSigninButton
                                    style={{ width: 192, height: 48, marginTop: 20 }}
                                    size={GoogleSigninButton.Size.Wide}
                                    color={GoogleSigninButton.Color.Dark}
                                    onPress={()=>{}}
                                    disabled={false} />
                                 </React.Fragment>
                                : <Text>Google Account Linked</Text>}
                            <View  style={{
                                marginTop:20
                            }}>
                            <Button title="Change Password" onPress={() => {
                                this.setState({changeType: 'password', modalVisible: true})
                            }} />
                            </View>

                            <View  style={{
                                marginTop:20
                            }}>
                            <Button title="Change Email" onPress={() => {
                                this.setState({changeType: 'email', modalVisible: true})
                            }} />
                            </View>
                            <View style={ styles.bottomView}>
                                <Button title="Log Out" onPress={() => {
                                    data.logout();
                                    this.props.navigation.navigate("Login", {})
                                }} />
                            </View>

                            <Modal
                                animationType="slide"
                                transparent={false}
                                visible={this.state.modalVisible}
                                onRequestClose={() => {
                                    this.setState({
                                        password1:'',
                                        password2:'',
                                        email:'',
                                        changeType: '', modalVisible: false})
                                }}>
                                <View style={{marginTop: 22}}>
                                    <View>
                                        {
                                            this.state.changeType == 'password' ? 
                                            <View>
                                                <TextInput placeholder="New Password" 
                                                textContentType="password" value={this.state.password1} 
                                                onChangeText={(text) => {this.setState({password1: text})}}
                                                />
                                                <TextInput
                                                style={{marginTop: 20}}
                                                placeholder="Confirm new Password" 
                                                textContentType="password" value={this.state.password2} 
                                                onChangeText={(text) => {this.setState({password2: text})}}
                                                />
                                            </View>
                                            : 
                                            <View>
                                                 <TextInput placeholder="New email" 
                                                textContentType="emailAddress" value={this.state.email} 
                                                onChangeText={(text) => {this.setState({email: text})}}
                                                />
                                            </View>
                                        }
                                    <View style={{marginTop: 20}}>
                                        <Button title={this.state.changeType == 'password' ? 
                                        "Change password": "Change email"} 
                                            onPress={() => {

                                            }}
                                        />
                                    </View>
                                    <View style={{marginTop: 20}}>
                                        <Button title="Cancel"
                                            onPress={() => {
                                                this.setState({
                                                    password1:'',
                                                    password2:'',
                                                    email:'',
                                                    changeType: '', modalVisible: false})
                                            }}
                                        />
                                    </View>
                                    </View>
                                </View>
                            </Modal>
                        </React.Fragment> 
                        : null 
                    }
                </View>
            )}
        </MusicStoreContext.Consumer>
    )
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