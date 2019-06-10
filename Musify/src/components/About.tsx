import React from 'react';
import { View, Text } from 'react-native';

export class About extends React.Component {
    render() {
       return (
        <View style={{
            flex: 1,
            justifyContent: 'center',
            alignItems: 'center'
        }}>
            <Text>Musify v1.0</Text>
        </View>
       )
    }
}