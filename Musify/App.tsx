import {createAppContainer} from 'react-navigation';
import AppViewer from './src/components/AppViewer';
import { AppProvider } from './src/components/Context/AppProvider';
import React from 'react';

global.Symbol = require('core-js/es6/symbol');
require('core-js/fn/symbol/iterator');

// collection fn polyfills
require('core-js/fn/map');
require('core-js/fn/set');
require('core-js/fn/array/find');

import NavigationService from './NavigationService';

const AppContainer = createAppContainer(AppViewer);

interface Props {
    ref: any;
    context: any;
}

export default class App extends React.Component<Props, {}> {
    render()
    {
        return (
            <AppProvider>
                <AppContainer
                ref={navigatorRef => {
                NavigationService.setTopLevelNavigator(navigatorRef);
                }}
            />
            </AppProvider>)
    }
}