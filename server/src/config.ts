let configuration: Configuration = {
  language: 'en',
  sparql: '',
  topics: {}
}

function configure(config: Configuration) {
  configuration = config
}

function getSPARQLEndpoint(): string {
  return configuration.sparql;
}

function getLanguage(): Language {
  return configuration.language;
}

function getTopicConfig(topic: URI): TopicConfig {
  return configuration.topics[topic]
}
