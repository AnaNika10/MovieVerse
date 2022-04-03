const path = require('path');

const HtmlWebPackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const ProgressBarPlugin = require('progress-bar-webpack-plugin');
const ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');

module.exports = {
	entry: './src/ts/index.tsx',
	devtool: 'inline-source-map',
	resolve: {
		extensions: ['.ts', '.tsx', '.js', '.jsx'],
		alias: {
			'root': path.resolve(__dirname, './src'),
			'style': path.resolve(__dirname, './src/style'),
		},
	},
	watch: false,
	output: {
		path: path.resolve(__dirname, 'public/app'),
		filename: 'app-bundle.js'
	},
	module: {
		rules: [
			{
				test: /\.ts(x?)$/,
				exclude: /node_modules/,
				use: [
					{
						loader: 'ts-loader',
						options: {
							happyPackMode: true
						}
					},
				]
			},
			{
				test: /\.scss$/u,
				use: [{
					loader: MiniCssExtractPlugin.loader,
				}, {
					loader: 'css-loader',
				}, {
					loader: 'sass-loader',
				}],
			}, {
				test: /\.(png|svg|jpg|gif)$/,
				use: [
					'file-loader',
				],
			},
			{
				test: /\.(jpe?g|gif|png|svg)$/i,
				use: [
					{
						loader: 'url-loader',
						options: {
							limit: 10000
						}
					}
				]
			},
		]
	},
	plugins: [
		new HtmlWebPackPlugin({
			template: './public/index.html',
		}),
		new MiniCssExtractPlugin({
			filename: 'app-style.css',
		}),
		new ProgressBarPlugin(),
		new ForkTsCheckerWebpackPlugin({
			typescript: {
				diagnosticOptions: {
					semantic: true,
					syntactic: true,
				},
			}
		}),
	]
};
