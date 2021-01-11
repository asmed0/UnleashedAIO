const initMyTls = require('mytls');

let user = 'customer-aycd107549plan1t1607150756635-cc-ee';
let pass = 'WCYDzRXMGcJ5qip';
let host = 'resi.proxies.aycd.io';
let port = 7777;
(async () => {

    const myTls = await initMyTls();
    for (i = 0; i < 1; i++) {
    const response = await myTls('https://www.courir.com/', {
      body: '',
      headers: { 
        'referer': 'https://www.courir.com/', 
        'user-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36', 
        'accept-language': 'en-US,en;q=0.9,sv;q=0.8',
        'accept':"*/*"
        },
      ja3: '771,255-49195-49199-49196-49200-49171-49172-156-157-47-53,0-10-11-13,23-24,0',
      proxy: `http://${user}:${pass}@${host}:${port}`
    });

    if(!await response.data.includes('/datadome/user_challenge/')){
        console.log(`Task [${i}] Passed datadome!`);
    }else{
        console.log(response.data)
        console.log(`Task [${i}] Got hit with captcha!`);
    }
  }
})();

