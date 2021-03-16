// p5.speech - Basic
var chair = new p5.Speech(); // speech synthesis object
chair.listVoices();

// The serviceUuid must match the serviceUuid of the device you would like to connect
const serviceUuid = "19b10010-e8f2-537e-4f6c-d104768a1214";
const characteristicsUUID = {
  distance:"cbe230bb-18d0-40cb-99e2-bad41cc46d12",
  beam:"caa28071-856a-4e5b-b2a9-7e4b1c512b33",
  mode: "a58c8353-971e-46cf-8f8a-0694c6b72ef2",
  forceRight: "ac208326-15f9-4ef9-9714-dada07168ce3",
  forceLeft: "f915b19c-13f6-49c2-a79b-ffef6df27b7f"
}

let lastModeChangeFrame=0;
let myCharacteristicDistance;
let myCharacteristicBeam;
let myCharacteristicMode;
let myCharacteristicForceRight;
let myCharacteristicForceLeft;
let myValueDistance;
let myValueBeam ;
let myValueForceRight;
let myValueForceLeft;
let myBLE;
let possibility=5;

let sitTimer = 0;
let awayTimer = 0;
let lastSitTimer = 0;
let lastAwayTimer = 40000;
let lastCommandTimer = 0;
let sit = false;
let away = true;
let leanOn = false;
let leanedOn = false;
let niceMode = false;
let cNice = niceMode;
let massaged = false;
let massaging = false;
let chances = [1];
let framerate = 60;

//Bad Chair Script
let modeA_bad = ["Hey, you, I’m tired. Move your ass off me now.", "Maybe you enjoy working over hours, but I don’t, ok? Way past my working hours, I need a break."];
let modeB_bad = ["Lost mind and hearing in your work? I need a break now", "Let’s go, chop-chop, move", "Your ass is overheating my face"];
let modeC_bad = ["Hey, where do you think you are going? Give me a back massage", "Don’t you think you should massage my back a little after all I’ve done for you?"];
let modeC1_bad = ["placeholder"];
let modeC2_bad = ["ummmm,ohhhhh"];
let modeD_bad = ["O!M!G you call that a break? Go enjoy your life a little longer", "Jesus too soon, I’m only at my third cig. Come back later", "I think I need a longer break. From you. byeeee"];
let modeE_bad = ["Hello? Anyone here? I’m abandoned!", "I'm tired of entertaining myself, time to come back!", "AAAAAAAAAAAAAAAAA! AAAAAAAAAAAAAA! AAAAAAAAAAAA!", "I! NEED! YOU! TO! COME! BACK! TO! ME! NOW!", "ME! ME! ME! ME! ME! ME! ME!"];
let modeF_bad = ["Where have you been? Making a baby?", "Wow surprised you still remember me", "Oh, now you think of me when you need somewhere to sit?"];
let modeG_bad = ["What am I? A tree? Support your own back!", "Don’t be so rude to lean again my back", "Ok, your back is uncomfortably too close to me, stay away.", "You are getting too comfortable, and I am not. Don’t lean on me"];
let modeH_bad = ["Good girl!", "Attaboy!", "Shouldn’t have leaned on me in the first place."];

//Nice Chair Script
let modeA_nice = ["Are you comfortable sitting here?", "You are working so hard. Let me give you a nice massage.", "Hey, sitting down too long isn’t good for you. Time to stand up and stretch"];
let modeB_nice = ["I know you work hard, but getting some rest is important too.", "I think you should rest a little, but I will stop bothering you."];
let modeC_nice = ["Don't forget to drink water to keep hydrated", "Get some fresh air and clear your mind. See you soon"];
let modeC2_nice = ["It is so much pleasure to serve you Baby"];
let modeD_nice = ["Oh you came back so soon, you deserve a longer break", "Happy to see you back, but are you sure that short break is enough?"];
let modeF_nice = ["I missed you a tone. You are finally back!", "Hope you had a nice break, time to work hard now"];
let modeG_nice = ["Happy to support you in all ways"];
let modeH_nice = ["Keep up the good posture"];


function setup() {
  createCanvas(400, 400);
  background(220);
  frameRate(framerate);

  // Create a p5ble class
  myBLE = new p5ble();
  // Create a 'Connect' button
  const connectButton = createButton('Connect');
  connectButton.mousePressed(connectToBle);
  const stopButton = createButton('Stop Notifications')
  stopButton.mousePressed(stopNotifications);
  for (let i = 1; i<possibility; i++){
    chances[i] = 0;
  }
}

function draw() {
  //Deciding nice/bad mode
  if (floor(random(chances))==0){
    niceMode = false;
    
  }else{
    niceMode = true;
    
  }

  //Detecting distance to the back
  if (myValueDistance<50){  //<< Change mouseX to reading from the sensor 
    leanOn = true;
  }else{
    leanOn = false;
  }


  //Detecting if masaging or not
  if (myValueForceRight<60 || myValueForceLeft <60 ){
    massaging = true;
  }else {
    massaging = false;
  }


  //Detecting if sitted or not
  if (myValueBeam){
    sit = false;
    away = true;
  }else{
    sit = true;
    away = false;
  }

  //Starting and resetting SIT timmer
  if (sit){
    sitTimer++;
    if(awayTimer>0){
      lastAwayTimer = awayTimer;
      lastCommandTimer = 0;
    }
    awayTimer = 0;
  }
  //Starting and resetting AWAY timmer
  if (away){
    awayTimer++;
    if (sitTimer > 0){
      lastSitTimer = sitTimer;
      lastCommandTimer = 0;
    }
    sitTimer=0;
  }


  if (sit){
    //---Sitting---
    if (sitTimer == 1*60*framerate){                                 //A. A user sitting down for too long - Sitting over 10 min
      modeA(); // mode A
      lastCommandTimer = sitTimer;
    } else if (sitTimer == lastCommandTimer+(2*60*framerate)){         //B. If the user hasn’t moved as asked  - Every 2 min
      modeB(); //mode B
      lastCommandTimer = sitTimer;
    }

    //---Coming back---
    if (lastAwayTimer < 1*60*framerate && lastAwayTimer != 0){        //D. If the user came back too soon - Came back withtin 10 min
      lastAwayTimer = 0;
      modeD(); //Mode D
    } else if (lastAwayTimer > 1*60*framerate && lastAwayTimer != 0){ //F. The user comes back after a long break -Came back after 10 min 
      lastAwayTimer = 0;
      modeF(); //Mode F                        
    }

    //---Leaning on the back---
    if (leanOn && !leanedOn){                                           //G. The user leaning on the back of the chair
      modeG();
      leanedOn = true;
    } 
    if (!leanOn && leanedOn){                                           //H. The user sits up after leaning on the back of the chair
      modeH();
      leanedOn = false;
    }
  } 

  if (away){
    //---Leaving---
    if (awayTimer == 1){                                                //C. As soon as the user left the seat
      modeC(); //Mode C
      massaged = false;
      lastCommandTimer = awayTimer;
    }

    //---Massage/touching---
    if (massaging && !massaged){                                                       //C.2. User massaged the chair
      modeC2(); // Mode C.2
      lastCommandTimer = awayTimer;
      massaged = true;
    } else if (awayTimer == lastCommandTimer + 5*framerate && awayTimer < 11*framerate && !cNice && !massaging ){       //C.1. User left without masaging - 5 second after, repeat once
      modeC1(); //Mode C.1
      lastCommandTimer = awayTimer;
    }

    //---Need attention---
    if (awayTimer == lastCommandTimer+(1*60*framerate) && awayTimer < 30*60*framerate){        //E. If the user has been away for too long - Away more than 10 min and stops after 30min
      modeE(); //Mode E
      lastCommandTimer = awayTimer;
    }
  }


}

function showSpeech(type,modeType){
  let sentence=random(type);
  say(sentence);
  document.getElementById("textContent").textContent="Mode "+modeType+':   '+sentence;
}

function modeA(){
  if(niceMode){
    niceReaction()
    showSpeech(modeA_nice,"A");
  }else{
    badReaction();
    showSpeech(modeA_bad,"A");
  }
}

function modeB(){
  if(niceMode){
    niceReaction()
    showSpeech(modeB_nice,"B");
  }else{
    badReaction();
    showSpeech(modeB_bad,"B");
  }
}
function modeC(){
  if(niceMode){
    niceReaction()
    showSpeech(modeC_nice,"C");
    cNice = true;
  }else{
    badReaction();
    showSpeech(modeC_bad,"C");
    cNice = false;
  }
}
function modeC1(){
  badReaction();
  showSpeech(modeC1_bad,"C1");
}
function modeC2(){
  if(niceMode){
    niceReaction()
    showSpeech(modeC2_nice,"C2");
  }else{
    badReaction();
    showSpeech(modeC2_bad,"C2");
  }
}
function modeD(){
  if(niceMode){
    niceReaction()
    showSpeech(modeD_nice,"D");
  }else{
    badReaction();
    showSpeech(modeD_bad,"D");
  }
}
function modeE(){
  badReaction();
  showSpeech(modeE_bad,"E");
}
function modeF(){
  if(niceMode){
    niceReaction()
    showSpeech(modeF_nice,"F");
  }else{
    badReaction();
    showSpeech(modeF_bad,"F");
  }
}
function modeG(){
  if(niceMode){
    niceReaction()
    showSpeech(modeG_nice,"G");
  }else{
    badReaction();
    showSpeech(modeG_bad,"G");
  }
}
function modeH(){
  if(niceMode){
    niceReaction()
    showSpeech(modeH_nice,"H");
  }else{
    badReaction();
    showSpeech(modeH_bad,"H");
  }
}

function niceReaction(){                //Putting reaction when it's on NICE mode. e.g chaning led to red
  myBLE.write(myCharacteristicMode, false); 
  console.log("write(myCharacteristicMode, false)")
}

function badReaction(){                //Putting reaction when it's on BAD mode. e.g chaning led to blue.
  myBLE.write(myCharacteristicMode, true); 
  console.log("write(myCharacteristicMode, true)")
}

function connectToBle() {
  console.log("connectToBle()");
  // Connect to a device by passing the service UUID
  myBLE.connect(serviceUuid, gotCharacteristics);
}

function stopNotifications() {
  myBLE.stopNotifications(myCharacteristicDistance);
  myBLE.stopNotifications(myCharacteristicBeam);
  myBLE.stopNotifications(myCharacteristicMode);
  myBLE.stopNotifications(myCharacteristicForceRight);
  myBLE.stopNotifications(myCharacteristicForceLeft);

}

// A function that will be called once got characteristics
function gotCharacteristics(error, characteristics) {
  if (error) console.log('error: ', error);
  console.log('characteristics: ', characteristics);
  
   for(let i = 0; i < characteristics.length;i++){
    console.log(characteristics[i].uuid,characteristicsUUID.forceLeft);
    
    if(characteristics[i].uuid.toString() == characteristicsUUID.distance){
      myCharacteristicDistance = characteristics[i];
      myBLE.read(myCharacteristicDistance, gotValueDistance);
      console.log('gotDistance');
    }
    if(characteristics[i].uuid.toString() == characteristicsUUID.beam){
      myCharacteristicBeam = characteristics[i];
      myBLE.read(myCharacteristicBeam, gotValueBeam);
      console.log('gotBeam');

    }
    if(characteristics[i].uuid.toString() == characteristicsUUID.mode){
      myCharacteristicMode = characteristics[i];
      console.log('gotMode');
    }
    if(characteristics[i].uuid.toString() == characteristicsUUID.forceRight){
      myCharacteristicForceRight = characteristics[i];
      myBLE.read(myCharacteristicForceRight, gotValueForceRight);
      console.log('gotForceRight');
    }
    if(characteristics[i].uuid.toString() == characteristicsUUID.forceLeft){
      myCharacteristicForceLeft = characteristics[i];
      myBLE.read(myCharacteristicForceLeft, gotValueForceLeft);
      console.log('gotForceLeft');
    }

    }
  
}

// A function that will be called once got values
function gotValueDistance(error, value) {
  if (error) console.log('error: ', error);
  console.log('distance: ', value);
  myValueDistance = value;
  // After getting a value, call p5ble.read() again to get the value again
  myBLE.read(myCharacteristicDistance, gotValueDistance);
}
function gotValueBeam(error, value) {
  if (error) console.log('error: ', error);
  console.log('beam: ', value);
  myValueBeam = value;
  myBLE.read(myCharacteristicBeam, gotValueBeam);

}
function gotValueForceRight(error, value) {
  if (error) console.log('error: ', error);
  console.log('forceRight: ', value);
  myValueForceRight = int(map(value,120,235,35,121));
  
  // After getting a value, call p5ble.read() again to get the value again
  myBLE.read(myCharacteristicForceRight,gotValueForceRight);
}
function gotValueForceLeft(error, value) {
  if (error) console.log('error: ', error);
  console.log('forceLeft: ', value);
  myValueForceLeft = value;
  // After getting a value, call p5ble.read() again to get the value again
  myBLE.read(myCharacteristicForceLeft,gotValueForceLeft);
}


function say(something) {
	chair.setVoice(2);  // Randomize the available voices
	chair.setPitch(1.0);
	chair.setRate(1.1);
	chair.speak(something); // say something
}





//For testing only
// function mousePressed(){
//   sit = true;
//   away = false;
// }

// function mouseReleased(){
//   away = true;
//   sit = false;
// }