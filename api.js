const express = require('express');
const adyenEncrypt = require('./api/custom')

const app = express();

app.get("/ftl_encryption/:num/:cvv/:month/:year/:holderName", (req,res,next) =>{
    res.status(200).json(getResponseJsonFromRequestJson(req))
});

module.exports = app;

const adyenKey = "10001|B6D07BD544BD5759FA13F1972F229EDFD76D2E39EC209797FC6A6A6B9F3388DD70255D83369FC6A10A9E3DDC90968345D62D73793B480C59458BA5C7E0EFBADC81DAE4060079064C556B4324C9EEA8D26EBB9011BBD8F769A6E463F2D078621ABC1432393FAECE489A68D85A0176A58E7292CB36E124305EB098DFB89C24AD58A27F7A21329DA2FE401199D5952C630340535785323E56F2B72AB8F18EA05DBA7A811C7A83B4B661358B6CCC338498F6BA10C9A16408FD33A231CC00EEE5A9397D92ECF3D616D44A687062833B5BF91EED57E3129B98B559192D65B787AE5A230A86D4ACF23C485318095DC4C589D1E990809BB2B74F0EDD3225FD3A64D89DD1";
const cseInstance = adyenEncrypt.createEncryption(adyenKey, {});


function formatCardNumber(value) {
    // remove all non digit characters
    var value = value.replace(/\D/g, '');
    var formattedValue;
    var maxLength;
    // american express, 15 digits
    if ((/^3[47]\d{0,13}$/).test(value)) {
      formattedValue = value.replace(/(\d{4})/, '$1 ').replace(/(\d{4}) (\d{6})/, '$1 $2 ');
      maxLength = 17;
    } else if((/^3(?:0[0-5]|[68]\d)\d{0,11}$/).test(value)) { // diner's club, 14 digits
      formattedValue = value.replace(/(\d{4})/, '$1 ').replace(/(\d{4}) (\d{6})/, '$1 $2 ');
      maxLength = 16;
    } else if ((/^\d{0,16}$/).test(value)) { // regular cc number, 16 digits
      formattedValue = value.replace(/(\d{4})/, '$1 ').replace(/(\d{4}) (\d{4})/, '$1 $2 ').replace(/(\d{4}) (\d{4}) (\d{4})/, '$1 $2 $3 ');
      maxLength = 19;
    }
    return formattedValue;
  }

function getResponseJsonFromRequestJson(req){
    let num = req.params.num;
    let cvv = req.params.cvv;
    let holderName = req.params.holderName;
    let monthv = req.params.month;
    let yearv = "20" +req.params.year;
    const timestamp = new Date();

    let cvc = {
        number:formatCardNumber(num),
        cvc: cvv,
        holderName: holderName,
        expiryMonth: monthv,
        expiryYear: yearv,
        generationtime: timestamp.toISOString()
    }
    let number = {
        number:formatCardNumber(num),
        cvc: cvv,
        holderName: holderName,
        expiryMonth: monthv,
        expiryYear: yearv,
        generationtime: timestamp.toISOString()
    }
    let month = {
        number:formatCardNumber(num),
        cvc: cvv,
        holderName: holderName,
        expiryMonth: monthv,
        expiryYear: yearv,
        generationtime: timestamp.toISOString()
    }

    let year = {
        number:formatCardNumber(num),
        cvc: cvv,
        holderName: holderName,
        expiryMonth: monthv,
        expiryYear: yearv,
        generationtime: timestamp.toISOString()
    }

    console.log(`Encrypted for ${holderName}`)

    let encNum = cseInstance.encrypt(number)
    let encCVV = cseInstance.encrypt(cvc)
    let encMonth = cseInstance.encrypt(month)
    let encYear = cseInstance.encrypt(year)

    let response = {
        num:encNum,
        cvv:encCVV,
        month:encMonth,
        year:encYear
    }
    return response;
}

function formatCardNumber(valueIn) {
    // remove all non digit characters
    let value = valueIn.replace(/\D/g, '');
    let formattedValue;
    let maxLength;
    // american express, 15 digits
    if ((/^3[47]\d{0,13}$/).test(value)) {
        formattedValue = value.replace(/(\d{4})/, '$1 ').replace(/(\d{4}) (\d{6})/, '$1 $2 ');
        maxLength = 17;
    } else if((/^3(?:0[0-5]|[68]\d)\d{0,11}$/).test(value)) { // diner's club, 14 digits
        formattedValue = value.replace(/(\d{4})/, '$1 ').replace(/(\d{4}) (\d{6})/, '$1 $2 ');
        maxLength = 16;
    } else if ((/^\d{0,16}$/).test(value)) { // regular cc number, 16 digits
        formattedValue = value.replace(/(\d{4})/, '$1 ').replace(/(\d{4}) (\d{4})/, '$1 $2 ').replace(/(\d{4}) (\d{4}) (\d{4})/, '$1 $2 $3 ');
        maxLength = 19;
    }
    return formattedValue;
}