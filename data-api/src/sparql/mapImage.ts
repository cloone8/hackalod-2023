export default (data: any[]) => {
  return data.map((row) => ({
    label: row.paintingname.value,
    desc: row.paintingdesc.value,
    url: row.paintingurl.value,
  }))
}
