import {createStackNavigator, createBottomTabNavigator, createAppContainer} from 'react-navigation';
import {Login} from './Login';
import { About } from './About';
import Ionicons from 'react-native-vector-icons/Ionicons';
import FontAwesome from 'react-native-vector-icons/FontAwesome';
import React from 'react';
import NavigationService from '../../NavigationService';
import { Register } from './Register';
import { MainView } from './MainApp/MainView';
import { UserInfo } from './Users/UserInfo';

export const MainApp = createBottomTabNavigator({
  "Search for Song": MainView,
  Account: UserInfo,
  About: About
},
{
  defaultNavigationOptions: ({ navigation }) => ({
    tabBarIcon: ({ focused, tintColor }) => {
      const { routeName } = navigation.state;
      let IconComponent = Ionicons;
      let iconName;
      if (routeName === 'Search for Song')
        iconName = `ios-search`;
      else if (routeName === 'Account')
        iconName = `ios-person`;
      else if (routeName == "About")
        iconName = `ios-information-circle`;
      else if (routeName == "Genre Chart") {
        iconName = `pie-chart`;
        IconComponent = FontAwesome;
      }

      return <IconComponent name={iconName as any} size={25} color={tintColor as any} />;
    },
  }),
  tabBarOptions: {
    activeTintColor: 'tomato',
    inactiveTintColor: 'gray',
  },
}
);

const MainAppContainer = createAppContainer(MainApp);

class MainComponent extends React.Component {
  render() {
    return (
      <MainAppContainer ref={navigatorRef => {
        NavigationService.setTopLevelNavigator(navigatorRef);
        }} />)
  }
}

const AppViewer = (initialScreen: string = "Login") => createStackNavigator({
  Login: {
    screen: Login,
    navigationOptions: {
      header: null
    }
  },
  Register : {
    screen: Register,
    navigationOptions: {
      header: null
    }
  },
  MainApp: {
    screen: MainApp,
    navigationOptions: {
      headerLeft: null,
      title: 'Musify',
      headerStyle: {
        backgroundColor: '#f4511e'
      },
      headerTintColor: '#fff',
      headerTitleStyle: {
        fontWeight: 'bold'
      }
    }
  }
}, {
  initialRouteName: initialScreen
});

export default AppViewer;