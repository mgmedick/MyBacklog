<template>
    <div>
        <div class="show-lg d-flex flex-column flex-shrink-0 p-3 position-absolute top-0 start-0" style="width: 280px; height: 100vh; margin-top: 63px;">
            <ul class="nav nav-pills flex-column mb-auto">
                <li v-for="(userList, userListIndex) in indexvm.userLists.filter(i => i.defaultListID)" key="userList.id" class="nav-item">
                    <a @click="selectedItemID = userList.id" href="#" class="nav-link text-dark" :class="{ 'active' : selectedItemID == userList.id }">
                        <font-awesome-icon :icon="getIconClass(userList.defaultListID)" size="lg" class="me-3"/>
                        <span>{{ userList.name }}</span>
                    </a>
                </li>
                <li class="border-top my-3"></li>
                <li v-for="(userList, userListIndex) in indexvm.userLists.filter(i => !i.defaultListID)" key="userList.id" class="nav-item">
                    <a @click="selectedItemID = userList.id" href="#" class="nav-link text-dark" :class="{ 'active' : selectedItemID == userList.id }">
                        <span>{{ userList.name }}</span>
                    </a>
                </li>    
                <li class="nav-item">
                    <a @click="onShowAddListClick" href="#" class="nav-link">
                        <font-awesome-icon icon="fa-solid fa-plus" size="lg" class="me-3"/>
                        <span>Add list</span>
                    </a>
                </li>
            </ul>
        </div>
        <div class="show-md row g-2 justify-content-center">
            <div class="btn-group">
                <button class="btn dropdown-toggle btn-primary d-flex align-items-center" type="button" data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                    <font-awesome-icon v-if="indexvm.userLists.find(i => i.id == selectedItemID)?.defaultListID" :icon="getIconClass(indexvm.userLists.find(i => i.id == selectedItemID)?.defaultListID)" size="lg"/>
                    <span class="mx-auto">{{ indexvm.userLists.find(i => i.id == selectedItemID)?.name }}</span>
                </button>
                <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton" style="width: 100%;">
                    <li v-for="(userList, userListIndex) in indexvm.userLists" :key="userList.id">
                        <a @click="selectedItemID = userList.id" class="dropdown-item" :class="{ 'active' : selectedItemID == userList.id }" href="#/" data-toggle="pill">{{ userList.name }}</a>
                    </li>
                    <li class="nav-item">
                        <a @click="onAddListModalClick" class="dropdown-item text-primary" href="#/" data-toggle="pill">
                            <font-awesome-icon icon="fa-solid fa-plus" size="lg" class="me-2"/>Add list
                        </a>
                    </li>                    
                </ul>
            </div>                
        </div>
        <user-list-games ref="userlistgames" :userid="indexvm.userID" :userlistid="selectedItemID" :userlists="indexvm.userLists" @delete="onDelete"></user-list-games>
        <div ref="addlistmodal" class="modal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Add List</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <form @submit.prevent="submitForm">
                        <div class="modal-body">
                                <input type="text" class="form-control" autocomplete="off" v-model="addListName" aria-describedby="spnListNameErrors" placeholder="List name">
                            
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                            <button type="submit" class="btn btn-primary" @click="onAddListClick">Add</button>
                        </div>
                    </form>                    
                </div>
            </div>
        </div>             
    </div>
</template>
<script>
    import { getFormData, setCookie } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers, sameAs } from '@vuelidate/validators';
    import { successToast, errorToast } from '../../js/common.js';
    
    export default {
        name: "UserLists",
        props: {
            indexvm: Object
        },
        data: function () {
            return {
                userLists: [],
                selectedItemID: 0,
                successMessages: [],
                errorMessages: [],
                addListModal: {},
                addListName: ''
            };
        },       
        watch: {},
        created: function () {
            var that = this;
            this.selectedItemID = this.indexvm.userLists[0].id;
        },
        mounted: function() {
            var that = this;
            that.addListModal = new Modal(that.$refs.addlistmodal);
            that.$refs.addlistmodal.addEventListener('hidden.bs.modal', event => {
                that.addListName = '';
            });             
        },
        methods: {
            getIconClass: function (id) {
                var iconClass = '';

                switch (id) {
                    case 1:
                        iconClass = 'fa-solid fa-list';
                        break;
                    case 2:
                        iconClass = 'fa-solid fa-inbox';
                        break;
                    case 3:
                        iconClass = 'fa-solid fa-play';
                        break;
                    case 4:
                        iconClass = 'fa-solid fa-check';
                        break;
                }

                return iconClass;
            },
            onShowAddListClick() {
                this.addListModal.show();
            },
            onAddListClick() {
                console.log("addlist")
            },
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);

                axios.post('/Home/AddUserList', formData)
                    .then((res) => {
                        if (res.data.success) {
                            successToast("List added");
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });     
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            }                                                            
        },
    };
</script>






