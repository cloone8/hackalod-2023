type Result = {
  name: string
  description: string
  links: [URI, URI][]
}

async function resolve(uri: URI): Promise<Result> {
  // use getLanguage() to set the language

  return {
    name: "",
    description: "",
    links: []
  }
}
