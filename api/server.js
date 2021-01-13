const http= require('http');
const app = require("../api")

const port =4050;

const server = http.createServer(app);

server.listen(port);