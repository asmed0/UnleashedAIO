const express = require('express');
const adyenEncrypt = require('');
const router = express.Router();

router.get('/:num/:cvv/:month/:year', (req,res,next) =>{
    res.status(200).json(getResponseJsonFromRequestJson(req))
})

