import React from "react";
import { View, StyleSheet, Text, Modal, TextInput, ToastAndroid } from "react-native";
import {Button} from 'react-native-elements';
import { MusicStoreContext } from "../Context/Context";
import { LoginButton, AccessToken, LoginManager } from "react-native-fbsdk";
import { GoogleSigninButton, GoogleSignin, statusCodes } from "react-native-google-signin";
import { UserService } from "./UserService";
import { User } from "./User";
import { AsyncStorageUtis } from "../AsnyStorageUtils";

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
                                                this.linkFbAccount(data1.accessToken.toString())
                                                LoginManager.logOut();
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
                                    onPress={this.linkGoogleAccount}
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
                                                secureTextEntry={true}
                                                textContentType="newPassword" value={this.state.password1} 
                                                onChangeText={(text) => {this.setState({password1: text})}}
                                                />
                                                <TextInput
                                                style={{marginTop: 20}}
                                                secureTextEntry={true}
                                                placeholder="Confirm new Password" 
                                                textContentType="newPassword" value={this.state.password2} 
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
                                                if (this.state.changeType == 'password')
                                                {
                                                    if (this.state.password1.length < 6)
                                                        ToastAndroid.show("Password length > 5", ToastAndroid.LONG);
                                                    else if (this.state.password1 != this.state.password2)
                                                        ToastAndroid.show("Passwords should be the same", ToastAndroid.LONG);
                                                    else {
                                                        UserService.changePassword({
                                                            username: data.user.username,
                                                            password: this.state.password1
                                                        }, data.user.token).then(() => {
                                                            ToastAndroid.show("Password changed successfully", ToastAndroid.LONG)
                                                            this.setState({
                                                                password1:'',
                                                                password2:'',
                                                                email:'',
                                                                changeType: '', modalVisible: false})
                                                        }, () => {
                                                            ToastAndroid.show("Something went wrong", ToastAndroid.LONG)
                                                        })
                                                    }
                                                }
                                                else {
                                                    if (!this.validate(this.state.email))
                                                        ToastAndroid.show("Invalid Email", ToastAndroid.LONG)
                                                    else {
                                                        let changeProfileObj = {
                                                            id: data.user.id,
                                                            username: data.user.username,
                                                            email: this.state.email,
                                                            facebookId: data.user.facebookId,
                                                            googleId: data.user.googleId,
                                                            userType: data.user.userType
                                                        };

                                                        let newUserDetails = {
                                                            ...changeProfileObj,
                                                            token: data.user.token,
                                                            refreshToken: data.user.refreshToken
                                                        }

                                                        UserService.changeProfile(
                                                            changeProfileObj, data.user.token).then(() => {
                                                                data.changeProfile(newUserDetails).then(() => {
                                                                    ToastAndroid.show("Email changed successfully", ToastAndroid.LONG)
                                                                    this.setState({
                                                                        password1:'',
                                                                        password2:'',
                                                                        email:'',
                                                                        changeType: '', modalVisible: false})
                                                                })
                                                        }, () => {
                                                            ToastAndroid.show("Something went wrong", ToastAndroid.LONG)
                                                        })
                                                    }
                                                }
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

    validate = (email: string) : boolean => {
        let reg = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/ ;
        return reg.test(email);
    }

    linkFbAccount(fbToken: string) {
        fetch('https://graph.facebook.com/v3.3/me?fields=email,name&access_token=' + fbToken)
        .then((response) => response.json())
        .then((json) => {
            let data = this.context;
            let user = data.user;
            user.facebookId = json.id;

            let changeProfileObj = {
                id: user.id,
                username: user.username,
                email: user.email,
                facebookId: json.id,
                googleId: data.user.googleId,
                userType: data.user.userType
            };

            UserService.changeProfile(changeProfileObj, user.token).then(() => {
                data.changeProfile(user).then(() => {
                    ToastAndroid.show("Facebook linked successfully", ToastAndroid.LONG)
                })
            }, () => {
                ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
            })
        })
        .catch(() => {
            ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
        })
    }

    linkGoogleAccount = async () => {
        try {
            await GoogleSignin.hasPlayServices();
            const userInfo = await GoogleSignin.signIn();

            let data = this.context;
            let user = data.user;
            user.googleId = userInfo.user.id;

            let changeProfileObj = {
                id: user.id,
                username: user.username,
                email: user.email,
                facebookId: data.user.facebookId,
                googleId: userInfo.user.id,
                userType: data.user.userType
            };
            
            UserService.changeProfile(changeProfileObj, user.token).then(() => {
                data.changeProfile(user).then(() => {
                    ToastAndroid.show("Google linked successfully", ToastAndroid.LONG)
                })
            }, () => {
                ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
            })

        } catch (error) {
          if (error.code === statusCodes.SIGN_IN_CANCELLED) {
            // ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
          } else if (error.code === statusCodes.IN_PROGRESS) {
            ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
          } else if (error.code === statusCodes.PLAY_SERVICES_NOT_AVAILABLE) {
            ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
          } else {
            ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
          }
        }
      };

};

UserInfo.contextType = MusicStoreContext;

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