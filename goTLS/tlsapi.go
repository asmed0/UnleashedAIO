package main

import (
    "fmt"
    "time"
    "context"
    "log"
    "net/http"
    "io/ioutil"
    "strings"
    "encoding/json"

    "github.com/refraction-networking/utls"
    "github.com/x04/cclient"
    "github.com/gofiber/fiber/v2"
)

var ctx = context.Background()

type returnStruct struct {
    Cookies[] string `json:"cookies"`
    Body string `json:"body"`
}

type PostStruct struct {
    Url string `json:"url"`
    Headers string `json:"headers"`
    Body string `json:"body"`
    Proxy string `json:"proxy"`
    RetType string `json:"retType"`
}

type GetStruct struct {
    Url string `json:"url"`
    Headers string `json:"headers"`
    Proxy string `json:"proxy"`
}

func main() {
	app := fiber.New()

        app.Post("/get/:authKey", func(c * fiber.Ctx) error {
        body := string(c.Body())
        return c.SendString(getRequest2(body))
    })

        app.Post("/post/:authKey", func(c * fiber.Ctx) error {

        body := string(c.Body())
        return c.SendString(postReq(body))
    })

        app.Listen(":7000")
}

func getRequest2(body string) string {
    var structA GetStruct
    json.Unmarshal([] byte(body), & structA)
    url := structA.Url
        //proxy := structA.Proxy

    headerList := strings.Split(structA.Headers, "=|=|=")

    req, err := http.NewRequest("GET", url, nil)
    if err != nil {
        log.Println(err)
    }

    for i := 0;
    i < len(headerList);
    i++{
        headerAA := strings.Split(headerList[i], ",,")
        req.Header.Set(headerAA[0], headerAA[1])
    }

    client, err := cclient.NewClient(tls.HelloChrome_Auto)
    if err != nil {
        log.Println(err)
    }

    resp, err := client.Do(req)
    if err != nil {
        log.Println(err)
    }

    dataBody, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        log.Println(err)
    }

    var cookieCollect[] string
    for _, cookie := range resp.Cookies() {
        cookieCollect = append(cookieCollect, cookie.Name + "=" + cookie.Value)
    }

    bytes, err := json.Marshal(returnStruct {
        Cookies: cookieCollect,
        Body: string(dataBody),
    })

    currentTime := time.Now()
    fmt.Println("[" + string(currentTime.Format("15:04:05") + "] Handled a GET Request"))

    return string(bytes)
}

func postReq(body string) string {
    var structA PostStruct
    json.Unmarshal([] byte(body), & structA)

    headerList := strings.Split(structA.Headers, "=|=|=")
    url := structA.Url
    postBody := structA.Body
        //proxy := structA.Proxy

    //retType := structA.RetType


    b := strings.NewReader(postBody)
    req, err := http.NewRequest("POST", url, b)
    if err != nil {
        log.Println(err)
    }

    req.Proto = "HTTP/2.0"
    req.ProtoMajor = 1
    req.ProtoMinor = 1

    for i := 0;
    i < len(headerList);
    i++{
        headerAA := strings.Split(headerList[i], ",,")
        headerAAA := strings.Replace(headerAA[1], "=|=", "", -1)
        req.Header.Set(headerAA[0], headerAAA)
    }

    client, err := cclient.NewClient(tls.HelloChrome_Auto)
    if err != nil {
        log.Println(err)
    }

    resp, err := client.Do(req)
    if err != nil {
        log.Println(err)
        return "Error Executing Request"
    }

    dataBody, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        fmt.Printf("error = %s \n", err)
    }

    var cookieCollect[] string
    for _, cookie := range resp.Cookies() {
        cookieCollect = append(cookieCollect, cookie.Name + "=" + cookie.Value)
    }

    bytes, err := json.Marshal(returnStruct {
        Cookies: cookieCollect,
        Body: string(dataBody),
    })

    currentTime := time.Now()
    fmt.Println("[" + string(currentTime.Format("15:04:05") + "] Handled a POST Request"))

    return string(bytes)
}
