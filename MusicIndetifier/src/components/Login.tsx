import React from 'react';
import {Component} from 'react';
import {
    Text,
    StyleSheet,
    View,
    TextInput,
    TouchableOpacity,
    ToastAndroid,
    AsyncStorage
} from 'react-native';
import { MusicStoreContext } from './Context/Context';
import { UserService } from './Users/UserService';
import { User } from './Users/User';
import NavigationService from '../../NavigationService';

interface Props {
}

interface States {
    username: string;
    password: string;
}

export class Login extends Component < Props,
States > {
    constructor(props : Props)
    {
        super(props);
        this.state = {
            username: 'avasiu',
            password: 'test'
        }
        this.redirect = this.redirect.bind(this);
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
                    data.user == null ?
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
                                this.redirect('MainApp');
                                /*UserService.login(this.state.username, this.state.password).then((user: User) => {
                                    data.login(user);
                                    AsyncStorage.setItem('user', JSON.stringify(user)).then(() => {
                                        this.redirect('MainApp');
                                    })
                                }, () => {
                                    ToastAndroid.show("Wrong Username/Password", ToastAndroid.LONG);
                                })*/
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
                    </View> : <View>
                        {this.redirect("Songs")}
                    </View> }
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
        backgroundColor: '#2c3e50'
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