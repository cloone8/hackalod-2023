export default (data: any[] | undefined) => {
  if (!data) {
    return [];
  }

  return data.map((row) => ({
    label: row.paintingname.value,
    desc: row.paintingdesc.value,
    url: row.paintingurl.value,
  }))
}
