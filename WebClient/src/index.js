function component () {
  const element = document.createElement('div')
  element.innerHTML = 'Hello webpack World'
  return element
}

document.body.appendChild(component())
