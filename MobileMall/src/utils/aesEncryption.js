// src/utils/aesEncryption.js
import CryptoJS from 'crypto-js'

// const key = CryptoJS.enc.Utf8.parse('12345678123456781234567812345678') // 32位密钥
// const iv = CryptoJS.enc.Utf8.parse('1234567812345678') // 16位IV

// 动态生成 Key 和 IV（每次加密时生成）
const generateKeyAndIV = () => {
  const key = CryptoJS.lib.WordArray.random(32) // 256位密钥（32字节）
  const iv = CryptoJS.lib.WordArray.random(16) // 128位IV（16字节）
  return { key, iv }
}

// AES加密
export const encrypt = (data) => {
  const { key, iv } = generateKeyAndIV()
  if (data) {
    const jsonData = JSON.stringify(data)
    const encrypted = CryptoJS.AES.encrypt(jsonData, key, {
      iv: iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7,
    })
    // return `{"EncryptedData": "${encrypted.toString()}","KEY":"${key}","IV":"${iv}"}`
    return {
      EncryptedData: encrypted.toString(),
      KEY: key.toString(CryptoJS.enc.Hex), // Key转为十六进制字符串
      IV: iv.toString(CryptoJS.enc.Hex), // IV转为十六进制字符串
    }
  } else {
    return {
      EncryptedData: '',
      KEY: key.toString(CryptoJS.enc.Hex), // Key转为十六进制字符串
      IV: iv.toString(CryptoJS.enc.Hex), // IV转为十六进制字符串
    }
  }
}

// AES解密
export const decrypt = (cipherText, keyHex, ivHex) => {
  const key = CryptoJS.enc.Hex.parse(keyHex)
  const iv = CryptoJS.enc.Hex.parse(ivHex)
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

// function generateSecureRandomString(length) {
//   const array = new Uint8Array(length) // 创建一个指定长度的Uint8Array
//   window.crypto.getRandomValues(array) // 填充数组
//   return Array.from(array, (byte) => byte.toString(16).padStart(2, '0')).join('') // 将每个字节转换为两位十六进制数并连接起来
// }
