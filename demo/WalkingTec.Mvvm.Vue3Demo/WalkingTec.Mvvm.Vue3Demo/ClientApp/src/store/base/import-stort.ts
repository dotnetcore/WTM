export default (serverUrl: string = "@/store/system/frameworkuser") => {
  import(serverUrl)
    .then(res => {})
    .catch(err => {
      console.log("err", err);
    });
  return;
};
