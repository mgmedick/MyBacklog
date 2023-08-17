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
                <tbody>
                    <tr v-for="(userList, userListIndex) in userLists.filter(i => i.defaultListID)">
                        <td><span>{{ userList.name }}</span></td>
                        <td>
                            <div class="d-flex">
                                <div class="ms-auto">
                                    <font-awesome-icon icon="fa-solid fa-square-pen" size="lg" class="me-3 fa-disabled" @click="onShowEditListClick($event, userList)"/>
                                    <font-awesome-icon icon="fa-solid fa-circle-xmark" size="xl" class="fa-disabled" style="color: #d9534f; background: radial-gradient(#fff 50%, transparent 50%);" @click="onShowDeleteListClick($event, userList)"/>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr v-for="(userList, userListIndex) in userLists.filter(i => !i.defaultListID)">
                        <td><span>{{ userList.name }}</span></td>
                        <td>
                            <div class="d-flex">
                                <div class="ms-auto">
                                    <font-awesome-icon icon="fa-solid fa-square-pen" size="lg" class="me-3" @click="onShowEditListClick($event, userList)"/>
                                    <font-awesome-icon icon="fa-solid fa-circle-xmark" size="xl" style="color: #d9534f; background: radial-gradient(#fff 50%, transparent 50%); cursor: pointer;" @click="onShowDeleteListClick($event, userList)"/>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>                       
                        <td>
                            <a @click="onShowAddListClick" href="#/" class="text-primary nav-link">
                                <font-awesome-icon icon="fa-solid fa-plus" size="lg" class="me-3"/>
                                <span>Add list</span>              
                            </a>
                        </td>
                        <td></td>                  
                    </tr>                                         
                </tbody>
            </table> 
        </div>           
        <div ref="addlistmodal" class="modal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Add List</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <save-user-list ref="addlist" :userlist="userList" @complete="onAddListComplete"></save-user-list>
                    </div>    
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="submit" class="btn btn-primary" @click="$refs.addlist.submitForm()">Submit</button>
                    </div>                                  
                </div>
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
                        <save-user-list ref="editlist" :userlist="userList" @complete="onEditListComplete"></save-user-list>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="submit" class="btn btn-primary" @click="$refs.editlist.submitForm()">Submit</button>
                    </div>                                     
                </div>
            </div>
        </div>
        <div ref="deletelistmodal" class="modal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
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
    import { successToast, errorToast } from '../../js/common.js';
    import axios from 'axios';
    import { Modal } from 'bootstrap';
   
    export default {
        name: "ManageUserLists",
        data() {
            return {
                userLists: [],
                userList: {},
                addListModal: {},
                editListModal: {},
                deleteListModal: {},
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

            that.addListModal = new Modal(that.$refs.addlistmodal, { backdrop: 'static' });
            that.editListModal = new Modal(that.$refs.editlistmodal, { backdrop: 'static' });
            that.deleteListModal = new Modal(that.$refs.deletelistmodal, { backdrop: 'static' });   

            that.$refs.addlistmodal.addEventListener('hidden.bs.modal', event => {
                that.userList = { userListID: 0, userListName: '' };
                that.$refs.addlist.v$.$reset();
            });      
            
            that.$refs.editlistmodal.addEventListener('hidden.bs.modal', event => {
                that.userList = { userListID: 0, userListName: '' };
                that.$refs.editlist.v$.$reset();
            });               
        },
        methods: {   
            loadData: function () {
                var that = this;
                this.loading = true;

                axios.get('/User/GetUserLists')
                    .then(res => {
                        that.userLists = res.data;

                        that.loading = false;
                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },                     
            onShowAddListClick() {
                this.userList = { id: 0, name: '' };
                this.addListModal.show();
            },                      
            onShowEditListClick(e, userList) {
                this.userList = userList;
                this.editListModal.show();    
            },
            onShowDeleteListClick(e, userList) {
                this.userList = userList;
                this.deleteListModal.show();  
            },                                      
            onDeleteListClick(e, userList) {
                var that = this;
                return axios.post('/User/DeleteUserList', null,{ params: { userListID: userList.id } })
                    .then((res) => {
                        that.deleteListModal.hide();

                        if (res.data.success) {
                            successToast("Successfully deleted <strong>" + userList.name + "</strong> list");                           
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                                
                        } 

                        that.loadData();
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            onAddListComplete: function (res) {
                this.addListModal.hide();

                if (res.data.success) {
                    successToast("Successfully added list");                           
                } else {
                    res.data.errorMessages.forEach(errorMsg => {
                        errorToast(errorMsg);                           
                    });                                
                }
                this.loadData();
            },                           
            onEditListComplete: function (res) {    
                this.editListModal.hide();
            
                if (res.data.success) {
                    successToast("Successfully saved list");                           
                } else {
                    res.data.errorMessages.forEach(errorMsg => {
                        errorToast(errorMsg);                           
                    });                                
                }
                this.loadData();
            }                      
        }    
    };
</script>


