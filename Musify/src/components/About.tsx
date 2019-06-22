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
            <Text style={{margin: 20, fontSize: 20, justifyContent: 'center'}}>About Musify</Text>
            <Text style={{margin:20, flexWrap: "wrap", textAlign: "justify"}}>
            Our mission is to unlock the potential of human creativity by giving a million creative artists the opportunity to live off their art and billions of fans the opportunity to enjoy and be inspired by it.
            </Text>
            <Text style={{marginTop: 200}}>Musify v1.0</Text>
            <Text style={{marginTop: 200}}>©Alex Vasiu 2019</Text>
        </View>
       )
    }
}