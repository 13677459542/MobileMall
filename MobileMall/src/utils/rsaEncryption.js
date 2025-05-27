import JSEncrypt from 'jsencrypt'

// 硬编码后端提供的公钥（完整PEM格式）
const BACKEND_PUBLIC_KEY = `-----BEGIN RSA PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3Pg+4SnPGW71Fm3tTxzzb0n1b41Pr1GuXvO4rA1VOX+SrVz+K8g98qyOF6RM2iOdaJBWat6IVjTuTLwVw+m2Q5aHlHSxkCX/RIBJNt9qIaOytbnn711ssecN00W+NFuiotLLeVP+ttSb28HGCXaGvNe1PFswDh0ZJMcMGY8JvC+h135nQNCQ0hG06vj0KzrmeIxRRNX1AQFIQz+7/hy/oJ/o0XJXrgGdH2e5e3Kox260G9QRruQZ4HGMrQ7VvaqeJ1v14qHLVI0ij3ycPeYJQ+tjXfjcheUppaTSuT2pdQRTY97nkPY//e3zXs4MKzlL3GAY+a3CfqjWRSjiOqTSywIDAQAB
-----END RSA PUBLIC KEY-----`

// 加密函数（传入明文和公钥字符串）
export const rsaEncrypt = (msg) => {
  const encryptor = new JSEncrypt()
  encryptor.setPublicKey(BACKEND_PUBLIC_KEY) // 设置公钥
  return encryptor.encrypt(msg.toString()) // 返回Base64格式密文
}

// 解密函数（通常仅后端使用）
export const rsaDecrypt = (encryptedMsg, privateKey) => {
  const decryptor = new JSEncrypt()
  decryptor.setPrivateKey(privateKey)
  return decryptor.decrypt(encryptedMsg)
}
