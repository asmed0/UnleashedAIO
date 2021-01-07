package main

import (
	"C"
	"encoding/json"
	"io/ioutil"
	"net/http"
	"strings"

	tls "github.com/refraction-networking/utls"
	"github.com/x04/cclient"
)

func main() {}

type returnStruct struct {
	Cookies []string `json:"cookies"`
	Body    string   `json:"body"`
}

//export getRequest
func getRequest(urlRaw *C.char, headersRaw *C.char, proxyRaw *C.char) *C.char {

	url := string(C.GoString(urlRaw))
	headers := string(C.GoString(headersRaw))
	proxy := string(C.GoString(proxyRaw))

	req, err := http.NewRequest("GET", url, nil)
	if err != nil {
		return C.CString("get error")
	}

	headerList := strings.Split(headers, "=|=|=")
	for i := 0; i < len(headerList); i++ {
		headerAA := strings.Split(headerList[i], ",,")
		req.Header.Set(headerAA[0], headerAA[1])
		//req.Header[headerAA[0]] = []string{headerAA[1]}
	}

	client, err := cclient.NewClient(tls.HelloChrome_Auto, proxy)
	if err != nil {
		return C.CString("newClient error")
	}

	resp, err := client.Do(req)
	if err != nil {
		return C.CString("do error")
	}

	dataBody, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return C.CString("nil error")
	}

	var cookieCollect []string
	for _, cookie := range resp.Cookies() {
		cookieCollect = append(cookieCollect, cookie.Name+"="+cookie.Value)
	}

	bytes, err := json.Marshal(returnStruct{
		Cookies: cookieCollect,
		Body:    string(dataBody),
	})
	if err != nil {
		return C.CString("error")
	}

	resp.Body.Close()
	return C.CString(string(bytes))
}

//export postRequest
func postRequest(urlRaw *C.char, headersRaw *C.char, bodyRaw *C.char, proxyRaw *C.char) *C.char {
	url := string(C.GoString(urlRaw))
	headers := string(C.GoString(headersRaw))
	body := string(C.GoString(bodyRaw))
	proxy := string(C.GoString(proxyRaw))

	b := strings.NewReader(body)
	req, err := http.NewRequest("POST", url, b)
	if err != nil {
		return C.CString("newreq error")
	}

	headerList := strings.Split(headers, "=|=|=")
	for i := 0; i < len(headerList); i++ {
		headerAA := strings.Split(headerList[i], ",,")
		req.Header.Set(headerAA[0], headerAA[1])
	}

	client, err := cclient.NewClient(tls.HelloChrome_Auto, proxy)
	if err != nil {
		return C.CString(string("Client Error"))
	}

	resp, err := client.Do(req)
	defer func() {
		if resp != nil {
			resp.Body.Close()
		}
	}()

	if err != nil {
		return C.CString("error1")
	}

	dataBody, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return C.CString("nil error2")
	}

	var cookieCollect []string
	for _, cookie := range resp.Cookies() {
		cookieCollect = append(cookieCollect, cookie.Name+"="+cookie.Value)
	}

	bytes, err := json.Marshal(returnStruct{
		Cookies: cookieCollect,
		Body:    string(dataBody),
	})
	if err != nil {
		return C.CString("error3")
	}

	resp.Body.Close()
	return C.CString(string(bytes))

}