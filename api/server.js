const http= require('http');
const app = require("../api")

const PORT = process.env.PORT || 3000;

const server = http.createServer(app);

server.listen(PORT);