const express = require('express');
const adyenEncrypt = require('./api/custom')

const app = express();

app.get("/ftl_encryption/:num/:cvv/:month/:year", (req,res,next) =>{
    res.status(200).json(getResponseJsonFromRequestJson(req))
});

module.exports = app;

const adyenKey = "10001|A237060180D24CDEF3E4E27D828BDB6A13E12C6959820770D7F2C1671DD0AEF4729670C20C6C5967C664D18955058B69549FBE8BF3609EF64832D7C033008A818700A9B0458641C5824F5FCBB9FF83D5A83EBDF079E73B81ACA9CA52FDBCAD7CD9D6A337A4511759FA21E34CD166B9BABD512DB7B2293C0FE48B97CAB3DE8F6F1A8E49C08D23A98E986B8A995A8F382220F06338622631435736FA064AEAC5BD223BAF42AF2B66F1FEA34EF3C297F09C10B364B994EA287A5602ACF153D0B4B09A604B987397684D19DBC5E6FE7E4FFE72390D28D6E21CA3391FA3CAADAD80A729FEF4823F6BE9711D4D51BF4DFCB6A3607686B34ACCE18329D415350FD0654D";
const cseInstance = adyenEncrypt.createEncryption(adyenKey, {});

function getResponseJsonFromRequestJson(req){
    let num = formatCardNumber(req.params.num);
    let cvv = req.params.cvv;
    let monthv = req.params.month;
    let yearv = "20" +req.params.year;

    let cvc = {
        cvc:cvv
    }
    let number = {
        number:num
    }
    let month = {
        month:monthv
    }

    let year = {
        year:yearv
    }


    cseInstance.validate(number);
    cseInstance.validate(cvc);
    cseInstance.validate(month);
    cseInstance.validate(year);

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
    return JSON.stringify(response);
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