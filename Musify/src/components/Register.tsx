import React, { Component } from 'react';
import { View, Text, TextInput, TouchableOpacity, ToastAndroid, StyleSheet, ActivityIndicator } from 'react-native';
import { UserService } from './Users/UserService';
import { User } from './Users/User';
import NavigationService from '../../NavigationService';

interface Props {
}

interface States {
    username: string;
    email: string;
    password1: string;
    password2: string;
    loading: boolean;
}

export class Register extends Component<Props, States> {
    constructor(props: Props) {
        super(props);
        this.state = {
            username: 'avasiu',
            email: 'test',
            password1: 'zanamea',
            password2: 'zanamea',
            loading: false
        }
    }

    render() {
        return (<View style={styles.container}>
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
                value={this.state.email}
                onChangeText={(e) => this.setState({email: e})}
                autoCapitalize="none"
                autoCorrect={false}
                keyboardType='email-address'
                returnKeyType="next"
                placeholder='Email'
                placeholderTextColor='rgba(225,225,225,0.7)'/>

            <TextInput
                style={styles.input}
                value={this.state.password1}
                onChangeText={(e) => this.setState({password1: e})}
                placeholder='Password'
                returnKeyType="next"
                placeholderTextColor='rgba(225,225,225,0.7)'
                secureTextEntry/>

            <TextInput
                style={styles.input}
                value={this.state.password2}
                onChangeText={(e) => this.setState({password2: e})}
                returnKeyType="go"
                placeholder='Retype Password'
                placeholderTextColor='rgba(225,225,225,0.7)'
                secureTextEntry/>

            <TouchableOpacity
                style={styles.buttonContainer}
                onPress={() => {
                    if (this.state.password1.length < 6)
                        ToastAndroid.show("Password length > 5", ToastAndroid.LONG);
                    else if (this.state.password1 != this.state.password2)
                        ToastAndroid.show("Passwords should be the same", ToastAndroid.LONG);
                    else
                        {
                            this.setState({loading: true})
                            UserService.register({
                            username: this.state.username,
                            email: this.state.email,
                            password: this.state.password1,
                            facebookId: "",
                            googleId: ""
                        }).then((user: User) => {
                            this.setState({loading: false})
                            ToastAndroid.show("User " + this.state.username + " was created.", ToastAndroid.LONG);
                            NavigationService.navigate('Login', {});
                        }, (error: any) => {
                            this.setState({loading: false})
                            ToastAndroid.show(error, ToastAndroid.LONG);
                        })
                    }
            }}>
                <Text style={styles.buttonText}>REGISTER</Text>
            </TouchableOpacity>
             
            {
                this.state.loading ? <ActivityIndicator size="large" color="#0000ff" /> : null
            }
        </View>)
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