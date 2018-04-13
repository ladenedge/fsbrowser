
function Item(data) {
    this.name = data.Name;
    this.fullPath = data.Path;
    this.mimeType = data.MimeType;
    this.isDir = data.MimeType === "inode/directory";
    this.size = formatSize(data.Size, this.isDir);
    this.mtime = formatTime(data.MTime);
    this.readOnly = data.ReadOnly;
    this.href = data.Self;
    this.hrefParent = data.Parent;
    this.hrefChildren = data.Children;
    this.children = ko.observableArray([]);
    this.icon = this.isDir ? '📂 ' : '📄 ';
    this.isRoot = this.fullPath.match(/\w:\\?$/);

    this.fetchChildren = function () {
        $.getJSON(this.hrefChildren, rsp => {
            var items = $.map(rsp, i => new Item(i, this))
                .sort((a, b) => b.isDir - a.isDir || a.name.localeCompare(b.name));
            this.children(items);
        });
    };

    this.parentName = () => {
        var parts = this.fullPath.split('\\').filter(p => p);
        parts.pop();
        return parts.pop();
    };

    this.parentPath = () => {
        var parts = this.fullPath.split('\\').filter(p => p);
        parts.pop();
        return parts.join('\\');
    };
}

var formatTime = time => {
    if (!time) return null;
    var date = new Date(parseInt(time.replace("/Date(", "").replace(")/", ""), 10));
    return date.toLocaleString("en-US");
};

var formatSize = (size, isDir) => {
    // thank you, https://stackoverflow.com/a/18650828/222481
    if (isDir) return '';
    if (size === 0) return '0 Bytes';
    var units = ['Bytes', 'KB', 'MB', 'GB'];
    var i = parseInt(Math.floor(Math.log(size) / Math.log(1024)));
    return Math.round(size / Math.pow(1024, i), 2) + ' ' + units[i];
};
