import React from 'react';
import { View, StyleSheet, Image } from 'react-native';
import { AppProvider } from './Context/AppProvider';
import AppViewer from './AppViewer';
import { AsyncStorageUtis } from './AsnyStorageUtils';
import { createAppContainer } from 'react-navigation';
import NavigationService from '../../NavigationService';

interface States {
    done: boolean;
    initialScreen: string;
}

interface Props {
    ref: any;
    context: any;
}

export class SplashScreen extends React.Component<Props, States> {

    constructor(props: Props) {
        super(props);
        this.state = {
            done: false,
            initialScreen: ''
        }
    }

    getViewer() : Promise<string> {
        return new Promise((resolve) => {
            AsyncStorageUtis.getItem("user").then((data: any) => {
                resolve(data != null ? 'MainApp' : 'Login')
            }, () => {
                resolve('Login')
            })
            .catch(() => {
                resolve('Login')
            })
        })
    }

    componentDidMount() {
        this.getViewer().then((value: string) => this.setState({initialScreen: value}))
        setTimeout(() => {
            this.setState({done: true})
        }, 1000)
    }

    render() {
        let AppContainer : any = null;
        if (this.state.done)
            AppContainer = createAppContainer(AppViewer(this.state.initialScreen));
        return (
        <AppProvider>
            {!this.state.done ? 
            <View style={styles.container}>
                <Image
                    style={styles.logo}
                    source={require('../images/logo.png')}
                />
            </View> :
                <AppContainer
                ref={(navigatorRef:any) => {
                NavigationService.setTopLevelNavigator(navigatorRef);
                }}
            />
            }
        </AppProvider>
        )
    }
}

const styles = StyleSheet.create({
    container: {
      justifyContent: 'center',
      alignItems: 'center',
      display: 'flex',
      flex: 1,
      flexDirection: 'column',
      backgroundColor: '#edf4ff'
    },
    logo: {
      width: 150,
      height: 150,
    },
  });