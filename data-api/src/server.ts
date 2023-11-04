import express, { Request, Response } from 'express';
import config from './config';
import query, { entityTypes } from './sparql';
import sharp from 'sharp';
import axios from 'axios';

import 'express-async-errors';

const app = express();

app.use(express.json());

app.get('/status', (_req: Request, res: Response) => {
  return res.status(200).json({ message: 'I\'m alive!'})
})

app.get('/entity/:entityType/:entityId', async (req: Request, res: Response) => {
  const { entityType, entityId } = req.params;

  if (!entityTypes.includes(entityType)) {
    return res.status(400).json({ error: `Invalid entity type ${entityType}. Expected one of [${entityTypes.join(', ')}]`})
  }

  const result = await query(entityType, entityId);

  return res.status(200).json(result);
})

app.get(`/image/:url`, async (req: Request, res: Response) => {
  const { url } = req.params;

  const response = await axios.get(url, { responseType: 'arraybuffer' });

  const contentType = response.headers['content-type'];
  const contentLength = response.headers['content-length'];

  if (!contentLength || parseInt(contentLength) > 100000000) {
    return res.status(400).end();
  }

  const resized = await sharp(response.data).resize(null, 1024).toBuffer()

  return res.status(200).contentType(contentType!).end(resized);
})

app.listen(config.port, () => console.log('server started'));
