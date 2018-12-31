const fs = require('fs');

class KeyPoint {
    constructor(coordinates, type, facing, available) {
        this.coordinates = coordinates;
        this.type = type;
        this.facing = facing;
        this.available = available;
    }
}

class Edge {
    constructor(coordinateA) {
        this.points = [coordinateA, null];
    }
    addPoint(coordinates) {
        if (this.points[0] !== null) {
            this.points[1] = coordinates;
        } else {
            this.points[0] = coordinates;
        }
    }
}

class RoomTemplate {
    constructor() {
        this.keypoints = [];
        this.roomType = null;
        this.numDoors = 0;
        this.origin = {x: 0, y: 0};
        this.doorScale = null;
        this.edges = [];
        this.currentEdge = null;
    }
    addOrigin(pointObject) { 
        this.origin = pointObject;
        this.edges.forEach(pointArray => {
            pointArray[0] -= this.origin.x;
            pointArray[1] -= this.origin.y;
        });
        this.keypoints.forEach(point => {
            point.coordinates = {x: point.coordinates.x - this.origin.x, y: point.coordinates.y - this.origin.y};
        });
    }
    addEdgePoint(pointObject) {
        pointObject = {x: pointObject.x - this.origin.x, y: pointObject.y - this.origin.y};
        if (this.currentEdge === null) {
            if (this.edges.length > 0) {
                //check to see if starting point of new edge is near to ending point of last edge
                if (distanceBetween(pointObject, this.edges[this.edges.length - 1][1]) > 20) {
                    // if so, use last edge point instead
                    pointObject = this.edges[this.edges.length - 1][1];
                }
            }
            this.currentEdge = new Edge(pointObject);
        } else {
            console.log(this.currentEdge)
            const dx = Math.abs(pointObject.x - this.currentEdge.points[0].x);
            const dy = Math.abs(pointObject.y - this.currentEdge.points[0].y);
            let useDx = 0;
            if (dx > dy && dx < 50) useDx = 1;
            else if (dy < 50) useDx = -1;
            switch (useDx) {
                case -1: pointObject = {x: this.currentEdge.points[0].x, y: pointObject.y}; break;
                case 1: pointObject = {x: pointObject.x, y: this.currentEdge.points[0].y}; break;
                default: break;
            }
            this.currentEdge.addPoint(pointObject);
            this.edges.push(this.currentEdge.points);
            this.currentEdge = null;
        }
    }
    addKeyPoint(pointObject, facing) {
        pointObject = {x: pointObject.x - this.origin.x, y: pointObject.y - this.origin.y};
        const available = this.keypoints.length > 0 || this.roomType !== "Foyer";
        const newPoint = new KeyPoint(pointObject, "Door", facing, available);
        this.keypoints.push(newPoint);
        this.numDoors ++;
    }
    verify() {
        return this.roomType !== null    &&
               this.numDoors > 0         &&
               this.doorScale !== null   &&
               this.edges.length > 0     &&
               this.keypoints.length > 0 &&
               this.currentEdge === null
    }
    set roomType(newVal) {
        if (this.keypoints.length > 0) {
            if (newVal === "Foyer") {
                this.keypoints[0].available = false;
            } else {
                this.keypoints[0].available = true;
            }
        }
    }
}


let templates = [];
let currentTemplate = new RoomTemplate();

function parseTemplateFile (filepath, mode="Overwrite") {
    fs.readFile(filepath, 'utf-8', function (err, data) {
        console.log(data);
    })
}

function distanceBetween(pointa, pointb) {
    return Math.sqrt(Math.pow(pointa.x + pointb.x, 2) + Math.pow(pointa.y + pointb.y, 2));
}

const addEdgePoint = function(pointObject) {
    currentTemplate.addEdgePoint(pointObject);
    console.log(`Adding ${pointObject.x}, ${pointObject.y} - have ${currentTemplate.edges.length} edges`);
}

const addKeyPoint = function(point, facing) {
    currentTemplate.addKeyPoint(point, facing);
    console.log(`Adding ${point.x}, ${point.y} [${facing}] - have ${currentTemplate.keypoints.length} keypoints`);
}

const setDoorScale = function(doorWidth) {
    currentTemplate.doorScale = doorWidth;
    console.log(`Setting door scale to ${doorWidth}`);
}
const setRoomType = function(type) {
    currentTemplate.roomType = type;
    console.log(`Setting room type to ${type}`);
}

const clear = function() {
    currentTemplate = new RoomTemplate();
    console.log(`Clearing`);
}

const addOrigin = function(point) {
    currentTemplate.addOrigin(point);
    console.log(`Setting origin to ${point.x}, ${point.y}`);
}

let addTemplate = function (template) {
    let i = 0;
    let foundMatch = false;
    let tempCategory = null; // this will be a list of door options unique for each room type
    while (i < templates.length && !foundMatch) {
        if (templates[i].RoomType === currentTemplate.roomType) {
            tempCategory = templates[i];
            foundMatch = true;
        }
        i++;
    }
    console.log(foundMatch);
    foundMatch = false;
    if (tempCategory === null) {
        tempCategory = {
            RoomType: currentTemplate.roomType,
            DoorOptions: [ {
                NumDoors: currentTemplate.numDoors,
                Templates: [ {
                    DoorScale: currentTemplate.doorScale,
                    Edges: currentTemplate.edges,
                    KeyPoints: currentTemplate.keypoints
                }]
            }]
        };
        templates.push(tempCategory);
        return;
    }
    i = 0;
    let curTemplates = null;
    let numEntrances = currentTemplate.roomType === "Foyer" ? 0 : 1; //support more than 1?
    while (i < tempCategory.length && !foundMatch) {
        if (tempCategory[i].NumDoors === numEntrances) {
            curTemplates = tempCategory[i];
            foundMatch = true;
        }
        i++;
    }
    console.log(foundMatch);
    if (curTemplates === null) {
        curTemplates = {
            NumEntrances: numEntrances,
            Templates: [ {
                DoorScale: currentTemplate.doorScale,
                Edges: currentTemplate.edges,
                KeyPoints: currentTemplate.keypoints
            }]
        };
        tempCategory.push(curTemplates);
        return;
    }
    curTemplates.Templates.push({
        DoorScale: currentTemplate.doorScale,
        Edges: currentTemplate.edges,
        KeyPoints: currentTemplate.keypoints
    });
    return;
}

const finishTemplate = function() {
    if (currentTemplate.verify()) {
        addTemplate(currentTemplate);
        currentTemplate = new RoomTemplate();
        return true;
    } else {
        alert(`Error: Tried to save invalid template`);
        console.error(currentTemplate);
        return false;
    }
}

function saveTemplateToFile(filename) {
    const json = JSON.stringify(templates);
    fs.writeFile(filename, json, function (err) {
        if (err !== null) console.error(err);
    });
}
