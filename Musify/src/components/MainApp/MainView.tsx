import React, { Component } from 'react';
import { MusicStoreContext } from '../Context/Context';
import { View, StyleSheet, Text } from 'react-native';

interface Props {
}

interface States {

}

export class MainView extends Component<Props, States> {
    constructor(props: Props) {
        super(props);
        this.state = {
           
        }
    }

    render() {
        return (
            <MusicStoreContext.Consumer>
                {(data: any) => (
                    <View style={styles.container}>
                        <Text>
                            test BOOOM
                        </Text>
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
        padding: 20
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