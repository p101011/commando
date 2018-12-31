const {app, BrowserWindow} = require('electron');
const {saveTemplateToFile} = require('./TemplateBuilder');

let mainWindow;

function createMainWindow() {
    mainWindow = new BrowserWindow({width: 1600, height: 1200})
    mainWindow.loadFile('TemplateBuilder.html');
    mainWindow.webContents.openDevTools()
    mainWindow.on('closed', function() {
        mainWindow = null;
    })
}

app.on('ready', createMainWindow);
app.on('window-all-closed', function() {
    app.quit();
})