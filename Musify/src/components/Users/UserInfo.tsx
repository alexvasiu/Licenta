import React from "react";
import { View, StyleSheet, Text, Button } from "react-native";
import { MusicStoreContext } from "../Context/Context";

interface Props {
    navigation : any;
}

export class UserInfo extends React.Component<Props, {}> {
    render = () => (
        <MusicStoreContext.Consumer>
            {(data: any) => (
                <View style={styles.container}>
                    {
                        data && data.user ?
                        <React.Fragment>
                            <Text>Username: {data.user.username}</Text>
                            <Button title="Log Out" onPress={() => {
                                data.logout();
                                this.props.navigation.navigate("Login", {})
                            }} />
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
      alignItems: 'center',
    }
  });