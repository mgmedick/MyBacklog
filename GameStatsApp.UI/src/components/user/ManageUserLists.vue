<template> 
    <div class="mx-auto">     
        <div v-if="loading">
            <div class="d-flex m-3">
                <div class="mx-auto">
                    <font-awesome-icon icon="fa-solid fa-spinner" spin size="2xl" />
                </div>
            </div>
        </div>   
        <div v-else class="table-responsive">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th scope="col">Active</th>
                        <th scope="col">Name</th>
                        <th scope="col" class="d-flex"><span class="ms-auto">Actions</span></th>
                    </tr>
                </thead>        
                <tbody>                                   
                    <tr v-for="(userList, userListIndex) in userLists" :class="{ 'highlighted' : selectedRow == userListIndex }" style="vertical-align: middle;">
                        <td class="col-1">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" v-model="userList.active" @change="onUpdateActive($event, userList)">
                            </div>
                        </td>
                        <td>
                            <span>{{ userList.name }}</span>
                        </td>                     
                        <td>
                            <div class="d-flex align-items-center">
                                <div class="ms-auto d-flex flex-column me-2">
                                    <font-awesome-icon v-if="userListIndex > 0" icon="fa-solid fa-angle-up" @click="onMoveUpClick(userListIndex)"/>                                
                                    <font-awesome-icon v-if="userListIndex < userLists.length - 1" icon="fa-solid fa-angle-down" @click="onMoveDownClick(userListIndex)"/>                                
                                </div>
                                <font-awesome-icon icon="fa-solid fa-square-pen" size="lg" class="me-2" :class="{ 'fa-disabled' : userList.defaultListID }" @click="onEditListClick($event, userList)"/>
                                <font-awesome-icon icon="fa-solid fa-circle-xmark" size="xl" :class="{ 'fa-disabled' : userList.defaultListID }" style="color: #d9534f; background: radial-gradient(#fff 50%, transparent 50%); cursor: pointer;" @click="onShowDeleteListClick($event, userList)"/>                                
                            </div>
                        </td>                        
                    </tr>                                      
                </tbody>
            </table>
            <div>                     
                <a @click="onAddListClick" href="#/" class="text-primary nav-link">
                    <font-awesome-icon icon="fa-solid fa-plus" size="lg" class="me-3"/>
                    <span>Add list</span>              
                </a>
            </div>            
        </div>           
        <div ref="editlistmodal" class="modal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Edit List</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <edit-user-list ref="editlist" :userlist="userList" @saved="onEditListSaved"></edit-user-list>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="submit" class="btn btn-primary" @click="$refs.editlist.submitForm">Submit</button>
                    </div>                                     
                </div>
            </div>
        </div>
        <div ref="deletelistmodal" class="modal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Delete List</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <p>Delete <strong>{{ userList?.name }}</strong> list?</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" class="btn btn-primary" @click="onDeleteListClick($event, userList)">Continue</button>
                    </div>
                </div>
            </div>
        </div>                                 
    </div>
</template>
<script>
    import { getFormData, successToast, errorToast } from '../../js/common.js';
    import axios from 'axios';
    import { Modal } from 'bootstrap';
   
    export default {
        name: "ManageUserLists",
        data() {
            return {
                userLists: [],
                userList: {},
                selectedRow: -1,
                throttleTimer: null,
                throttleDelay: 300,                
                loading: false
            }
        },
        computed: {
        },
        created: function() {
            this.loadData();
        },        
        mounted: function () {
            var that = this;

            document.addEventListener('click', that.handleClickOutside);            
        },
        methods: {   
            loadData: function () {
                var that = this;
                this.loading = true;

                axios.get('/User/ManageUserLists')
                    .then(res => {
                        that.userLists = res.data;

                        that.loading = false;
                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            onEditListSaved(result) {
                Modal.getInstance(this.$refs.editlistmodal).hide();
                this.loadData();
            },                                
            onAddListClick() {
                this.userList = { id: 0, name: '', active: true, sortOrder: 0 };
                new Modal(this.$refs.editlistmodal, { backdrop: 'static' }).show();
            },                      
            onEditListClick(e, userList) {
                this.userList = {...userList};
                new Modal(this.$refs.editlistmodal, { backdrop: 'static' }).show();
            },            
            onShowDeleteListClick(e, userList) {
                this.userList = userList;
                new Modal(this.$refs.deletelistmodal, { backdrop: 'static' }).show(); 
            },                                      
            onDeleteListClick(e, userList) {
                var that = this;
                return axios.post('/User/DeleteUserList', null,{ headers: { '__csrf': document.getElementsByName('__csrf')[0].value },
                                                                 params: { userListID: userList.id } })
                    .then((res) => {
                        Modal.getInstance(that.$refs.deletelistmodal).hide();

                        if (res.data.success) {
                            successToast("Deleted <strong>" + userList.name + "</strong> list");                           
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                                
                        } 

                        that.loadData();
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            onUpdateActive(e, userList) {
                var that = this;
                return axios.post('/User/UpdateUserListActive', null,{ headers: { '__csrf': document.getElementsByName('__csrf')[0].value },
                                                                       params: { userListID: userList.id, active: userList.active } })
                    .then((res) => {
                        if (res.data.success) {

                            successToast((userList.active ? "Activated" : "Inactivated") + " <strong>" + userList.name + "</strong> list");                           
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                                
                        }                                                                                                     
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },                                               
            onMoveUpClick: function (rowIndex) {
                var item = this.userLists[rowIndex];
                var toIndex = rowIndex - 1;
                this.userLists.splice(rowIndex, 1);
                this.userLists.splice(toIndex, 0, item);
                this.selectedRow = toIndex;
                this.updateUserListSortOrderDelay();
            },
            onMoveDownClick: function (rowIndex) {
                var item = this.userLists[rowIndex];
                var toIndex = rowIndex + 1;
                this.userLists.splice(rowIndex, 1);
                this.userLists.splice(toIndex, 0, item);
                this.selectedRow = toIndex;
                this.updateUserListSortOrderDelay();           
            },
            updateUserListSortOrderDelay() {
                var that = this;

                clearTimeout(that.throttleTimer);
                    that.throttleTimer = setTimeout(function () {
                            that.updateUserListSortOrders();
                }, that.throttleDelay);     
            },
            updateUserListSortOrders() {
                var that = this;
                var userListIDs = that.userLists.map(i => { return i.id });
                var formData = getFormData({ userListIDs: userListIDs });
                var config = { headers: { '__csrf': document.getElementsByName('__csrf')[0].value } };

                return axios.post('/User/UpdateUserListSortOrders', formData, config)
                    .then((res) => {
                        if (res.data.success) {
                            successToast("Updated user lists");                           
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                                
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            handleClickOutside(event) {
                if (!(this.$el == event.target || this.$el.contains(event.target))) {
                    this.isOpen = false;
                    this.isFocus = false;
                    this.selectedRow = -1;
                }
            },                                     
        }    
    };
</script>


