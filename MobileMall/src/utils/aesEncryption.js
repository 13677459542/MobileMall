// src/utils/aesEncryption.js
import CryptoJS from 'crypto-js'

const key = CryptoJS.enc.Utf8.parse('12345678123456781234567812345678') // 32位密钥
const iv = CryptoJS.enc.Utf8.parse('1234567812345678') // 16位IV

// AES加密
export const encrypt = (data) => {
  const jsonData = JSON.stringify(data)
  const encrypted = CryptoJS.AES.encrypt(jsonData, key, {
    iv: iv,
    mode: CryptoJS.mode.CBC,
    padding: CryptoJS.pad.Pkcs7,
  })
  // return encrypted.toString()
  return `{"EncryptedData": "${encrypted.toString()}","IV":"${iv}"}`
}

// AES解密
export const decrypt = (cipherText) => {
  const decrypted = CryptoJS.AES.decrypt(cipherText, key, {
    iv: iv,
    mode: CryptoJS.mode.CBC,
    padding: CryptoJS.pad.Pkcs7,
  })
  const plainText = decrypted.toString(CryptoJS.enc.Utf8)
  return JSON.parse(plainText)
}

// Base64字符转普通字符串
export const base64ToString = (base64) => {
  // 使用 atob() 方法将Base64字符串解码为普通字符串
  const binaryString = window.atob(base64)
  // 将普通字符串转换为UTF-8格式，创建Uint8Array再转为字符串
  const bytes = new Uint8Array(binaryString.length)
  for (let i = 0; i < binaryString.length; i++) {
    bytes[i] = binaryString.charCodeAt(i)
  }
  const decoder = new TextDecoder('utf-8')
  return decoder.decode(bytes) // 返回解码后的字符串
}
