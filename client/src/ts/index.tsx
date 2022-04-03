// External libraries
import React from 'react';
import ReactDOM from 'react-dom';

// External UI Components
import { ThemeProvider } from '@mui/material/styles';

// UI components
import App from './components/app-component';

// Theme
import theme from './theme-provider';

// Style
import 'style/index.scss';

ReactDOM.render(
	<ThemeProvider theme={theme}>
		<App />
	</ThemeProvider>,
	document.getElementById('app'),
);
