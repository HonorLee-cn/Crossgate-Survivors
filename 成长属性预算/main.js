const fs = require('fs');

let baseBalue = {
    HP  : 20,
    MP  : 20,
    ATK : 20,
    DEF : 10,
    SPD : 40,
    EXP : 50
}

let cv = [5,4.5,4,3.5,3,2.5,2,1.5,1,0.5]

let baseGrow = {
    HP: 0.06,
    MP: 0.02,
    ATK: 0.02,
    DEF: 0.03,
    SPD: 0.01,
    EXP: 0.1
};

let secGrow = {
    HP: 0.045,
    MP: 0.1,
    ATK: 0.02,
    DEF: 0.03,
    SPD: 0.01,
    EXP: 0.5
};

function Calculate(lastValue,rate){
    return lastValue + lastValue*rate;
}

let outputCsv = "属性类型,基础数值,角色属性,成长率,1级,2级,3级,4级,5级,6级,7级,8级,9级,10级,11级,12级,13级,14级,15级,16级,17级,18级,19级,20级,21级,22级,23级,24级,25级,26级,27级,28级,29级,30级,31级,32级,33级,34级,35级,36级,37级,38级,39级,40级,41级,42级,43级,44级,45级,46级,47级,48级,49级,50级\n";

for(let key in baseBalue){
    let base = baseBalue[key];
    for(let attr of cv){
        let value = attr * base;
        let grow = baseGrow[key];
        outputCsv += key + "," + base + "," + attr + "," + grow + ",";
        for(let i=1;i<=50;i++){
            value = parseInt(Calculate(value,grow));
            outputCsv += value + ",";
        }
        outputCsv += "\n";
    }
}

fs.writeFile("人物成长属性参考.csv",outputCsv,function(err){
    if(err){
        console.log(err);
    }
    console.log("success");
});

// Path: output2.csv
outputCsv = "属性类型,基础数值,角色属性,成长率,1级,2级,3级,4级,5级,6级,7级,8级,9级,10级,11级,12级,13级,14级,15级,16级,17级,18级,19级,20级,21级,22级,23级,24级,25级,26级,27级,28级,29级,30级,31级,32级,33级,34级,35级,36级,37级,38级,39级,40级,41级,42级,43级,44级,45级,46级,47级,48级,49级,50级\n";

for(let key in baseBalue){
    let base = baseBalue[key];
    for(let attr of cv){
        let value = attr * base;
        let grow = secGrow[key];
        outputCsv += key + "," + base + "," + attr + "," + grow + ",";
        for(let i=1;i<=50;i++){
            value = parseInt(Calculate(value,grow));
            outputCsv += value + ",";
        }
        outputCsv += "\n";
    }
}

fs.writeFile("怪物成长属性参考.csv",outputCsv,function(err){
    if(err){
        console.log(err);
    }
    console.log("success");
});
