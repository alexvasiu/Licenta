import React from 'react';
import {Component} from 'react';
import {
    Text,
    StyleSheet,
    View,
    TextInput,
    TouchableOpacity,
    ToastAndroid,
    ActivityIndicator
} from 'react-native';
import { MusicStoreContext } from './Context/Context';
import { UserService } from './Users/UserService';
import { User } from './Users/User';
import NavigationService from '../../NavigationService';
import { AsyncStorageUtis } from './AsnyStorageUtils';
import { LoginButton, AccessToken, LoginManager } from 'react-native-fbsdk';
import { GoogleSignin, GoogleSigninButton, statusCodes } from 'react-native-google-signin';

interface Props {
}

interface States {
    username: string;
    password: string;
    loading: boolean;
}

export class Login extends Component < Props,
States > {
    constructor(props : Props)
    {
        super(props);
        this.state = {
            username: '',
            password: '',
            loading: false
        }
        this.redirect = this.redirect.bind(this);
    }
    
    componentDidMount()
    {
        GoogleSignin.configure({
            scopes: ['https://www.googleapis.com/auth/drive.readonly'], // what API you want to access on behalf of the user, default is email and profile
            webClientId: '389641282720-bdhmm8ko1kk1n85t0gkufm6aut0brjtc.apps.googleusercontent.com', // client ID of type WEB for your server (needed to verify user ID and offline access)
            offlineAccess: true, // if you want to access Google API on behalf of the user FROM YOUR SERVER
            hostedDomain: '', // specifies a hosted domain restriction
            loginHint: '', // [iOS] The user's ID, or email address, to be prefilled in the authentication UI if possible. [See docs here](https://developers.google.com/identity/sign-in/ios/api/interface_g_i_d_sign_in.html#a0a68c7504c31ab0b728432565f6e33fd)
            forceConsentPrompt: true, // [Android] if you want to show the authorization prompt at each login.
            accountName: '', // [Android] specifies an account name on the device that should be used
        });
    }

    redirect(path: string) {
        NavigationService.navigate(path, {});
        return true;
    }

    render()
    {
        return (
            <MusicStoreContext.Consumer>
                {(data: any) =>
                    <View style={styles.container}>
                        <TextInput
                            style={styles.input}
                            value={this.state.username}
                            onChangeText={(e) => this.setState({username: e})}
                            autoCapitalize="none"
                            autoCorrect={false}
                            returnKeyType="next"
                            placeholder='Username'
                            placeholderTextColor='rgba(225,225,225,0.7)'/>

                        <TextInput
                            style={styles.input}
                            value={this.state.password}
                            onChangeText={(e) => this.setState({password: e})}
                            returnKeyType="go"
                            placeholder='Password'
                            placeholderTextColor='rgba(225,225,225,0.7)'
                            secureTextEntry/>

                        <TouchableOpacity
                            style={styles.buttonContainer}
                            onPress={() => {
                                this.setState({loading: true})
                                UserService.login(this.state.username, this.state.password).then((user: User) => {
                                    data.login(user);
                                    AsyncStorageUtis.setItem('user', JSON.stringify(user)).then(() => {
                                        this.setState({loading: false})
                                        this.redirect('MainApp');
                                    })
                                }, () => {
                                    this.setState({loading: false})
                                    ToastAndroid.show("Wrong Username/Password", ToastAndroid.LONG);
                                })
                        }}>
                            <Text style={styles.buttonText}>LOGIN</Text>
                        </TouchableOpacity>

                        <View style={{
                                display: 'flex',
                                flexDirection: 'row',
                                justifyContent: 'center'
                            }}>
                            <Text
                                style={{
                                color: 'white',
                                paddingTop: 25,
                                paddingEnd: 5
                            }}>
                                Don't have an account ?
                            </Text>

                            <Text style={{
                                color: 'white',
                                paddingTop: 25
                            }}
                                onPress=
                                {() => { this.redirect("Register") }}>Register</Text>
                        </View>
                        

                        {
                            this.state.loading ? <ActivityIndicator size="large" color="#0000ff" /> : null
                        }

                        <View style={styles.socials}>
                            <LoginButton
                                readPermissions={['email']}
                                onLoginFinished={
                                    (error, result) => {
                                        if (error)
                                            ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
                                        else if (result.isCancelled) {
                                            //ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
                                        } else {
                                            AccessToken.getCurrentAccessToken().then(
                                                (data1: any) => {
                                                    this.initUser(data1.accessToken.toString())
                                                    LoginManager.logOut();
                                                }
                                            )
                                        }
                                    }
                                }
                                onLogoutFinished={() => console.warn("logout.")}/>
                                <GoogleSigninButton
                                    style={{ width: 192, height: 48, marginTop: 20 }}
                                    size={GoogleSigninButton.Size.Wide}
                                    color={GoogleSigninButton.Color.Dark}
                                    onPress={this.signIn}
                                    disabled={false} />
                        </View>
                    </View> }
            </MusicStoreContext.Consumer>
        )
    }

    signIn = async () => {
        try {
            await GoogleSignin.hasPlayServices();
            const userInfo = await GoogleSignin.signIn();
            let userGoogle = {
                googleId: userInfo.user.id,
                email: userInfo.user.email
            };

            UserService.loginGoogle(userGoogle).then((user: User) => {
                this.context.login(user);
                AsyncStorageUtis.setItem('user', JSON.stringify(user)).then(() => {
                    this.setState({loading: false})
                    this.redirect('MainApp');
                })
            }, () => {
                this.setState({loading: false})
                ToastAndroid.show("Wrong Username/Password", ToastAndroid.LONG);
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

    initUser(token: string) {
        fetch('https://graph.facebook.com/v3.3/me?fields=email,name&access_token=' + token)
        .then((response) => response.json())
        .then((json) => {
            let fbUser = {
                facebookId: json.id,
                email: json.email
            };
            UserService.loginFacebook(fbUser).then((user: User) => {
                this.context.login(user);
                AsyncStorageUtis.setItem('user', JSON.stringify(user)).then(() => {
                    this.setState({loading: false})
                    this.redirect('MainApp');
                })
            }, () => {
                this.setState({loading: false})
                ToastAndroid.show("Wrong Username/Password", ToastAndroid.LONG);
            })
        })
        .catch(() => {
            ToastAndroid.show("Something went wrong", ToastAndroid.LONG);
        })
    }
}

Login.contextType = MusicStoreContext;

const styles = StyleSheet.create({
    container: {
        justifyContent: 'center',
        flexDirection: 'column',
        flex: 1,
        padding: 20,
        backgroundColor: '#2c3e50'
    },
    socials: {
        marginTop: 20,
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignSelf: 'center'
    },
    input: {
        height: 40,
        backgroundColor: 'rgba(225,225,225,0.2)',
        marginBottom: 10,
        padding: 10,
        color: '#fff'
    },
    logo: {
        position: 'absolute',
        width: 300,
        height: 100
    },
    buttonContainer: {
        backgroundColor: '#2980b6',
        paddingVertical: 15
    },
    buttonText: {
        color: '#fff',
        textAlign: 'center',
        fontWeight: '700'
    }
});