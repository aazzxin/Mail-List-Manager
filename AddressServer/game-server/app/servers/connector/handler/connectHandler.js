var fs = require('fs');
var mysql = require('mysql');
var pool = mysql.createPool({
    host: '127.0.0.1',
    user: 'root',
    password: '',
    port: '3306',
    database: 'addressserver'
});

module.exports = function (app) {
    return new Handler(app);
};

var Handler = function (app) {
    this.app = app;
};

var indexJson = require('../../../../data/idIndex.json');
var dataRoot = './data/idIndex.json';

/**
 * register.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.register = function (msg, session, next) {
    if (msg.userId === null)
    {
        next(null, { code: 500, msg: '请输入用户名' });
        return;
    }

    //mysql操作
    pool.getConnection(function (err, connection) {
        var userAddSql = 'INSERT INTO userinfo(userId,nickName,password) VALUES(?,?,?)';
        var userAddSql_Params = [msg.userId, "Temp", msg.password];
        connection.query(userAddSql, userAddSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '注册失败' });
            }
            else {
                var contactIndexJson = {};
                contactIndexJson.contactId = 0;
                contactIndexJson.removeContactId = [];
                contactIndexJson.typeId = 1;
                contactIndexJson.removeTypeId = [];
                //Json存储
                indexJson[msg.userId] = contactIndexJson;
                fs.writeFile(dataRoot, JSON.stringify(indexJson), function (err) {
                    if (err) throw err;
                    console.log('It\'s saved!');
                });
                next(null, { code: 200, msg: '注册成功', userId: msg.userId, nickName: msg.userId, password: msg.password });
            }
        });
        connection.release();
    });
};
/**
 * login.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.login = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '请输入用户名' });
        return;
    }
    //mysql操作
    pool.getConnection(function (err, connection) {
        var userSelectSql = 'SELECT * from userinfo WHERE userId=?';
        var userSelectSql_Params = [msg.userId];
        connection.query(userSelectSql, userSelectSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
            }
            else {
                if (result.length === 0) {
                    next(null, { code: 500, msg: '账号或密码错误' });
                }
                else if (msg.password != result[0].password) {
                    next(null, { code: 500, msg: '账号或密码错误' });
                }
                else
                    next(null, {
                        code: 200, msg: 'login success', userId: result[0].userId, nickName: result[0].nickName,
                        password: result[0].password
                    });
            }
        });
        connection.release();
    });
};
/**
 * modify password.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.modifyPassword = function (msg, session, next) {
    //mysql操作
    pool.getConnection(function (err, connection) {
        var userUpdateSql = 'UPDATE userinfo SET password=? WHERE userId=?';
        var userUpdateSql_Params = [msg.password, msg.userId];
        connection.query(userUpdateSql, userUpdateSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
            }
            else {
                next(null, { code: 200, msg: '修改成功' });
            }
        });
        connection.release();
    });
};
/**
 * modify nickName.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.modifyNickName = function (msg, session, next) {
    //mysql操作
    pool.getConnection(function (err, connection) {
        var userUpdateSql = 'UPDATE userinfo SET nickName=? WHERE userId=?';
        var userUpdateSql_Params = [msg.nickName, msg.userId];
        connection.query(userUpdateSql, userUpdateSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
            }
            else {
                next(null, { code: 200, msg: '修改成功' });
            }
        });
        connection.release();
    });
};
/**
 * delete user.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.deleteUser = function (msg, session, next) {
    var isSuccess = true;
    //mysql操作
    pool.getConnection(function (err, connection) {
        var contacttypeDeleteSql = 'DELETE FROM contacttype WHERE userId=?';
        var contacttypeDeleteSql_Params = [msg.userId];
        connection.query(contacttypeDeleteSql, contacttypeDeleteSql_Params, function (err) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
                isSuccess = false;
            }
        });
        var typesetDeleteSql = 'DELETE FROM typeset WHERE userId=?';
        var typesetDeleteSql_Params = [msg.userId];
        connection.query(typesetDeleteSql, typesetDeleteSql_Params, function (err) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
                isSuccess = false;
            }
        });
        var contactinfoDeleteSql = 'DELETE FROM contactinfo WHERE userId=?';
        var contactinfoDeleteSql_Params = [msg.userId];
        connection.query(contactinfoDeleteSql, contactinfoDeleteSql_Params, function (err) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
                isSuccess = false;
            }
        });
        var userinfoDeleteSql = 'DELETE FROM userinfo WHERE userId=?';
        var userinfoDeleteSql_Params = [msg.userId];
        connection.query(userinfoDeleteSql, userinfoDeleteSql_Params, function (err) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
                isSuccess = false;
            }
        });
        connection.release();
    });
    if (isSuccess) {
        //Json存储
        delete indexJson[msg.userId];
        fs.writeFile(dataRoot, JSON.stringify(indexJson), function (err) {
            if (err) throw err;
            console.log('It\'s saved!');
        });
        next(null, { code: 200, msg: '删除成功' });
    }
};

 /**
 * searchContact.
 */
Handler.prototype.searchContact = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '用户无效' });
        return;
    }
    //mysql操作
    pool.getConnection(function (err, connection) {
        var contactSelectSql = "SELECT contactId,nickName from contactinfo WHERE userId=?";
        var contactSelectSql_Params = [msg.userId];
        if (msg.typeName != "") {
            contactSelectSql = "SELECT contactinfo.contactId, contactinfo.nickName from contactinfo,typeset,contacttype ";
            contactSelectSql += "WHERE typeset.typeId=contacttype.typeId AND typeset.userId=contacttype.userId AND ";
            contactSelectSql += "contacttype.contactId=contactinfo.contactId AND contacttype.userId=contactinfo.userId AND ";
            contactSelectSql += "typeset.userId=? AND typeset.typeName=?";
            contactSelectSql_Params.push(msg.typeName);
            if (msg.sex != "") {
                contactSelectSql += " AND contactinfo.sex=?";
                contactSelectSql_Params.push(msg.sex);
            }
            if (msg.nickName != "") {
                contactSelectSql += " AND contactinfo.nickName=?";
                contactSelectSql_Params.push(msg.nickName);
            }
        }
        else {
            if (msg.sex != "") {
                contactSelectSql += " AND sex=?";
                contactSelectSql_Params.push(msg.sex);
            }
            if (msg.nickName != "") {
                contactSelectSql += " AND nickName=?";
                contactSelectSql_Params.push(msg.nickName);
            }
        }
        connection.query(contactSelectSql, contactSelectSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
            }
            else {
                next(null, {
                    code: 200, msg: 'search success', contacts: result
                });
            }
        });
        connection.release();
    });
};

/**
 * readContact.
 */
Handler.prototype.readContact = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '用户无效' });
        return;
    }
    //mysql操作
    pool.getConnection(function (err, connection) {
        var addressSelectSql = 'SELECT nickName,sex,tel,email,remarks from contactinfo WHERE userId=? AND contactId=?';
        var addressSelectSql_Params = [msg.userId, msg.contactId];
        connection.query(addressSelectSql, addressSelectSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
            }
            else {
                var typeSelectSql = 'SELECT typeset.typeId,typeset.typeName from contactinfo,typeset,contacttype ';
                typeSelectSql += "WHERE contactinfo.userId=? AND contactinfo.contactId=? ";
                typeSelectSql += "AND typeset.typeId=contacttype.typeId AND typeset.userId=contacttype.userId AND ";
                typeSelectSql += "contacttype.contactId=contactinfo.contactId AND contacttype.userId=contactinfo.userId";
                var typeSelectSql_Params = [msg.userId, msg.contactId];
                connection.query(typeSelectSql, typeSelectSql_Params, function (err, types) {
                    if (err) {
                        console.log('[INSERT ERROR] - ', err.message);
                        next(null, { code: 500, msg: '连接数据库错误' });
                    }
                    else {
                        next(null, {
                            code: 200, msg: 'search success', nickName: result[0].nickName, sex: result[0].sex,
                            tel: result[0].tel,email: result[0].email, types: types, remarks: result[0].remarks
                        });
                    }
                });
            }
        });
        connection.release();
    });
};

/**
 * addContact.
 */
Handler.prototype.addContact = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '用户无效' });
        return;
    }

    //获取新建联系人的编号
    var contactId = indexJson[msg.userId].contactId;
    var removeId = indexJson[msg.userId].removeContactId;
    var newContactId = contactId;
    var replaceId = null;
    for (var i = 0; i < removeId.length; i++) {
        if (newContactId > removeId[i])
        {
            newContactId = removeId[i];
            replaceId = i;
        }
    }

    var isSuccess = true;
    //mysql操作
    pool.getConnection(function (err, connection) {
        var contactInsertSql = 'INSERT INTO contactinfo VALUES (?,?,?,?,?,?,?)';
        var contactInsertSql_Params = [newContactId, msg.userId, msg.nickName, msg.sex, msg.tel, msg.email, msg.remarks];
        connection.query(contactInsertSql, contactInsertSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
            }
            else {
                for (var i = 0; i < msg.types.length; i++) {
                    var typeInsertSql = 'INSERT INTO contacttype VALUES (?,?,?)';
                    var typeInsertSql_Params = [newContactId, msg.userId, msg.types[i].typeId];
                    connection.query(typeInsertSql, typeInsertSql_Params, function (err, result) {
                        if (err) {
                            console.log('[INSERT ERROR] - ', err.message);
                            next(null, { code: 500, msg: '连接数据库错误' });
                            isSuccess = false;
                        }
                    });
                }
                if (isSuccess) {
                    //创建成功后，修改用户联系人编号json数据
                    if (replaceId != null) {
                        removeId.splice(replaceId, 1);
                    }
                    else {
                        contactId++;
                    }
                    //Json存储
                    indexJson[msg.userId].contactId = contactId;
                    indexJson[msg.userId].removeContactId = removeId;
                    fs.writeFile(dataRoot, JSON.stringify(indexJson), function (err) {
                        if (err) throw err;
                        console.log('It\'s saved!');
                    });

                    var newContact = {};
                    newContact.contactId = newContactId;
                    newContact.nickName = msg.nickName;
                    var contactArray = [newContact];

                    next(null, {
                        code: 200, msg: '添加联系人成功', contacts: contactArray
                    });
                }
            }
        });
        connection.release();
    });
};
/**
 * dropContact.
 */
Handler.prototype.dropContact = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '用户无效' });
        return;
    }
    //mysql操作
    pool.getConnection(function (err, connection) {
        var contactTypeDeleteSql = 'DELETE FROM contacttype WHERE userId=? AND contactId=?';
        var contactTypeDeleteSql_Params = [msg.userId, msg.contactId];
        connection.query(contactTypeDeleteSql, contactTypeDeleteSql_Params);
        var contactDeleteSql = 'DELETE FROM contactinfo WHERE userId=? AND contactId=?';
        var contactDeleteSql_Params = [msg.userId, msg.contactId];
        connection.query(contactDeleteSql, contactDeleteSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '删除失败' });
            }
            else {
                //json为remove类别添加编号
                var removeId = indexJson[msg.userId].removeContactId;
                removeId.push(msg.contactId);
                indexJson[msg.userId].removeContactId = removeId;
                fs.writeFile(dataRoot, JSON.stringify(indexJson), function (err) {
                    if (err) throw err;
                    console.log('It\'s saved!');
                });
                next(null, { code: 200, msg: '删除联系人成功' });
            }
        });
        connection.release();
    });
};
/**
 * updateContact.
 */
Handler.prototype.updateContact = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '用户无效' });
        return;
    }
    var isSuccess = true;
    //mysql操作
    pool.getConnection(function (err, connection) {
        var addressUpdateSql = 'UPDATE contactinfo SET sex=?,nickName=?,tel=?,email=?,remarks=? WHERE userId=? AND contactId=?';
        var addressUpdateSql_Params = [msg.sex, msg.nickName, msg.tel, msg.email, msg.remarks, msg.userId, msg.contactId];
        connection.query(addressUpdateSql, addressUpdateSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
                isSuccess = false;
            }
            else {
                for (var i = 0; i < msg.deleteTypes.length; i++) {
                    var typeDeleteSql = 'DELETE FROM contacttype WHERE contactId=? AND userId=? AND typeId=?';
                    var typeDeleteSql_Params = [msg.contactId, msg.userId, msg.deleteTypes[i]];
                    connection.query(typeDeleteSql, typeDeleteSql_Params, function (err, result) {
                        if (err) {
                            console.log('[INSERT ERROR] - ', err.message);
                            next(null, { code: 500, msg: '连接数据库错误' });
                            isSuccess = false;
                        }
                    });
                }
                for (var i = 0; i < msg.newTypes.length; i++) {
                    var typeInsertSql = 'INSERT INTO contacttype VALUES (?,?,?)';
                    var typeInsertSql_Params = [msg.contactId, msg.userId, msg.newTypes[i]];
                    connection.query(typeInsertSql, typeInsertSql_Params, function (err, result) {
                        if (err) {
                            console.log('[INSERT ERROR] - ', err.message);
                            next(null, { code: 500, msg: '连接数据库错误' });
                            isSuccess = false;
                        }
                    });
                }
            }
        });
        connection.release();
    });
    if (isSuccess)
        next(null, { code: 200, msg: '编辑联系人成功' });
};

/**
 * showType.
 */
Handler.prototype.showType = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '用户无效' });
        return;
    }
    //mysql操作
    pool.getConnection(function (err, connection) {
        var typeSelectSql = 'SELECT typeId,typeName FROM typeset WHERE userId=?';
        var typeSelectSql_Params = [msg.userId];
        connection.query(typeSelectSql, typeSelectSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '连接数据库错误' });
            }
            else {
                next(null, { code: 200, msg: '更新信息成功', types: result });
            }
        });
        connection.release();
    });
};
/**
 * addType.
 */
Handler.prototype.addType = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '用户无效' });
        return;
    }
    //获取新类别的编号
    var typeId = indexJson[msg.userId].typeId;
    var removeId = indexJson[msg.userId].removeTypeId;
    var newTypeId = typeId;
    var replaceId = null;
    for (var i = 0; i < removeId.length; i++) {
        if (newTypeId > removeId[i]) {
            newTypeId = removeId[i];
            replaceId = i;
        }
    }
    //mysql操作
    pool.getConnection(function (err, connection) {
        var typeInsertSql = 'INSERT INTO typeset VALUES(?,?,?)';
        var typeInsertSql_Params = [newTypeId, msg.userId, msg.typeName];
        connection.query(typeInsertSql, typeInsertSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '存在重复类别名' });
            }
            else {
                //创建成功后，修改用户联系人编号json数据
                if (replaceId != null) {
                    removeId.splice(replaceId, 1);
                }
                else {
                    typeId++;
                }
                //Json存储
                indexJson[msg.userId].typeId = typeId;
                indexJson[msg.userId].removeTypeId = removeId;
                fs.writeFile(dataRoot, JSON.stringify(indexJson), function (err) {
                    if (err) throw err;
                    console.log('It\'s saved!');
                });

                next(null, { code: 200, msg: '添加类别成功' });
            }
        });
        connection.release();
    });
};
/**
 * updateType.
 */
Handler.prototype.updateType = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '用户无效' });
        return;
    }
    //mysql操作
    pool.getConnection(function (err, connection) {
        var typeUpdateSql = 'UPDATE typeset SET typeName=? WHERE userId=? AND typeId=?';
        var typeUpdateSql_Params = [msg.typeName, msg.userId, msg.typeId];
        connection.query(typeUpdateSql, typeUpdateSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '更新失败，请确认无重复类别名' });
            }
            else {
                next(null, { code: 200, msg: '更新类别成功' });
            }
        });
        connection.release();
    });
};
/**
 * dropType.
 */
Handler.prototype.dropType = function (msg, session, next) {
    if (msg.userId === null) {
        next(null, { code: 500, msg: '用户无效' });
        return;
    }
    //mysql操作
    pool.getConnection(function (err, connection) {
        var contactTypeDeleteSql = 'DELETE FROM contacttype WHERE userId=? AND typeId=?';
        var contactTypeDeleteSql_Params = [msg.userId, msg.typeId];
        connection.query(contactTypeDeleteSql, contactTypeDeleteSql_Params);
        var typeDeleteSql = 'DELETE FROM typeset WHERE userId=? AND typeId=?';
        var typeDeleteSql_Params = [msg.userId, msg.typeId];
        connection.query(typeDeleteSql, typeDeleteSql_Params, function (err, result) {
            if (err) {
                console.log('[INSERT ERROR] - ', err.message);
                next(null, { code: 500, msg: '删除失败' });
            }
            else {
                //json为remove类别添加编号
                var removeId = indexJson[msg.userId].removeTypeId;
                removeId.push(msg.typeId);
                indexJson[msg.userId].removeTypeId = removeId;
                fs.writeFile(dataRoot, JSON.stringify(indexJson), function (err) {
                    if (err) throw err;
                    console.log('It\'s saved!');
                });
                next(null, { code: 200, msg: '删除类别成功' });
            }
        });
        connection.release();
    });
};