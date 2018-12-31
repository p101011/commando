const blueprintLoader = document.getElementById("blueprintLoader");
let currentMode = "Edge";
const dialog = require('electron').remote.dialog;
const { webFrame } = require('electron');

let lastMouseDownPoint = {x: 0, y: 0};
let doorMeasure = null;
let loadedBlueprint = false;
let imageBounds = {x: 0, y: 0};

let mousePosUpdate = function(event) {
    document.getElementsByClassName("coords")[0].innerHTML = `${event.pageX}, ${event.pageY}`;
}

let updateImage = function() {
    const canvas = document.getElementById("imageCanvas");
    canvas.width = this.width;
    canvas.height = this.height;
    imageBounds = {x: this.width, y: this.height};
    const ctx = canvas.getContext('2d');
    ctx.drawImage(this, 0, 0);
}

let handleNewBlueprint = function(event) {
    loadedBlueprint = false;
    const reader = new FileReader();
    reader.onload = function(event){
        loadedBlueprint = true;
        const img = new Image();
        img.onload = updateImage;
        img.src = event.target.result;
    }
    reader.readAsDataURL(event.target.files[0]);  
}

let reset = function() {
    clear();
}

let handleZoom = function(event) {
    //called with every click of scroll wheel

    if (!event.ctrlKey) return;
    event.preventDefault();
    const zoomDelta = event.deltaY / -1000;
    webFrame.setZoomFactor(webFrame.getZoomFactor() + zoomDelta);
}

let handleUserPick = function(event) {
    if (!loadedBlueprint) return;
    const point = {x: event.pageX, y: event.pageY};
    if (point.x > imageBounds.x || point.y > imageBounds.y) return;
    lastMouseDownPoint = point;
    if (currentMode === "Edge") {
        addEdgePoint(point);
    } else if (currentMode === "Scale") {
        if (doorMeasure === null) {
            doorMeasure = point;
        }
        else {
            const doorWidth = Math.sqrt(Math.pow(doorMeasure.x - point.x, 2) + Math.pow(doorMeasure.y - point.y, 2))
            setDoorScale(Math.round(doorWidth));
            doorMeasure = null;
        }
    } else if (currentMode === "Origin") {
        addOrigin(point);
    }
    //door setting requires the user draw a line from center of door outwards, no handling here
}

let handleUserDraw = function(event) {
    if (currentMode !== "Door" || !loadedBlueprint) return;
    const upPoint = {x: event.pageX, y: event.pageY};
    if (upPoint.x > imageBounds.x || upPoint.y > imageBounds.y) return;
    const dx = upPoint.x - lastMouseDownPoint.x;
    const dy = upPoint.y - lastMouseDownPoint.y;
    const rad = Math.atan2(dy, dx) + Math.PI;
    const rounded = Math.round(2 * rad / Math.PI);
    let facing;
    switch (rounded) {
        case 0:
        case 4: facing = "West"; break;
        case 1: facing = "North"; break;
        case 2: facing = "East"; break;
        case 3: facing = "South"; break;
        default: console.error(`${rounded} is not an even number! (got ${rad / Math.PI} radians)`); return;
    }
    addKeyPoint(lastMouseDownPoint, facing);
}

const save = function() {
    return finishTemplate();
}

const saveClose = function() {
    const success = save();
    if (success) dialog.showSaveDialog(function (filename) {
        if (filename === undefined) return;
        saveTemplateToFile(filename);
    });
}

let loadNewTemplate = function() {
    dialog.showOpenDialog({ filters: [ {
        name: 'JSON', extensions: ['json']
    }]}, function (filenames) {
        if (filenames === undefined) return;
        parseTemplateFile(filenames[0]);
    });
}

let switchMode = function(mode) {
    console.log(mode);
    currentMode = mode;
}

let sanitizeInput = function(string) {
    // look ma, i care
    return string;
}

let nameRoomType = function() {
    setRoomType(document.getElementById("roomType").textContent);
}

document.onmousemove = mousePosUpdate;
blueprintLoader.onchange = handleNewBlueprint;
document.onmousedown = handleUserPick;
document.onmouseup = handleUserDraw;
document.onwheel = handleZoom;