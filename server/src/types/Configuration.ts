type TopicConfig = {
  images: URI[]
  topics: URI[]
}

type Language = 'en' | 'nl' | 'de'

type Configuration = {
  sparql: string
  topics : Record<URI, TopicConfig>
  language: Language
}
