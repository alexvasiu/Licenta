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
import { GoogleSignin, GoogleSigninButton } from 'react-native-google-signin';

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
                                <GoogleSigninButton
                                    style={{ width: 192, height: 48, marginTop: 20 }}
                                    size={GoogleSigninButton.Size.Wide}
                                    color={GoogleSigninButton.Color.Dark}
                                    onPress={()=>{}}
                                    disabled={false} />
                        </View>
                    </View> }
            </MusicStoreContext.Consumer>
        )
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