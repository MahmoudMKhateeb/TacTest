// .lintstagedrc.js
module.exports = {
  // '../**/*.cs': (filenames) =>
  //   filenames.map((filename) => `dotnet format  ../\\\\aspnet-core\\\\TACHYON.Web.sln --include ${filename.replace('../aspnet-core/', '')}`),
  '*.{js,ts,tsx,html,css}': 'prettier --write',
};
