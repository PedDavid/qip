import fetch from 'isomorphic-fetch'

function extractBase64ImageFromImageSrc (imageSrc) {
  return imageSrc.slice(imageSrc.indexOf(',') + 1, imageSrc.length)
}

export function uploadImage (imageSrc) {
  const headers = new window.Headers({
    'Authorization': 'Client-ID 925c1bb08a795be',
    'Content-Type': 'application/json'
  })
  const image = extractBase64ImageFromImageSrc(imageSrc)
  const body = JSON.stringify({image})
  const options = { method: 'POST', headers, body }
  return fetch('https://api.imgur.com/3/image', options)
    .then(resp => {
      if (resp.ok) {
        return resp.json()
      }
      throw new Error(resp.status)
    })
}
